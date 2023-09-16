namespace CommonModels.Services
{
    using CommonModels;
    using CommonModels.Hubs;
    using CommonModels.Managers;
    using Microsoft.AspNetCore.SignalR;
    using System;
    using System.Diagnostics;
    using System.Text;
    using System.Timers;

    public class GameService : IDisposable
    {
        private const string CMDProcess = "cmd.exe";
        private const string SteamProcess = "steamcmd";
        private const string SteamLoginPrompt = "+login";
        private const string SteamForceInstall = "+force_install_dir";
        private const string SteamAppUpdate = "+app_update";

        public string GameServerLocation { get; }
        public string? CurrentlyRunningGame { get; set; }
        private readonly IHubContext<MessagingHub> _chatHubContext;
        private string? _gameId;

        private Process? _steamCmdProcess;
        private Process? _gameServerProcess;

        private readonly AutoResetEvent _steamCmdInputAllowedEvent = new AutoResetEvent(false);

        public GameService(
            IHubContext<MessagingHub> chatHubContext, string serverLocation)
        {
            _chatHubContext = chatHubContext;
            GameServerLocation = serverLocation;

            SteamLogin.Username = JsonManager.GetPropertyValue("Username");

            Timer batchTimer = new Timer(500);
            batchTimer.Elapsed += OnBatchTimerElapsed;
            batchTimer.AutoReset = true;
            batchTimer.Start();
        }

        public void StartAndUpdateSteamCMD(Game game)
        {
            _gameId = game.GameId;

            // Create a new ProcessStartInfo object and set the necessary properties
            ProcessStartInfo steamCmdProcessInfo = new ProcessStartInfo(CMDProcess)
            {
                UseShellExecute = false, // Set to false to redirect standard input, output, and error
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true // Set to true to hide the cmd window
            };

            // Start the process
            _steamCmdProcess = new Process();
            _steamCmdProcess.StartInfo = steamCmdProcessInfo;
            _steamCmdProcess.OutputDataReceived += SteamCmdProcess_OutputDataReceived;
            _steamCmdProcess.ErrorDataReceived += SteamCmdProcess_ErrorDataReceived;
            _steamCmdProcess.Start();

            _steamCmdProcess.BeginOutputReadLine();
            _steamCmdProcess.BeginErrorReadLine();

            Directory.CreateDirectory(Path.Combine(GameServerLocation, game.GameLocation));

            SendCommand(_steamCmdProcess, $"{SteamProcess} {SteamForceInstall} {Path.Combine(GameServerLocation, game.GameLocation)} {SteamLoginPrompt} {SteamLogin.Username} {SteamAppUpdate} {game.GameId}");

            _steamCmdInputAllowedEvent.WaitOne();

            SendCommand(_steamCmdProcess, "quit");

            _steamCmdProcess.Close();
            _steamCmdProcess = null;
        }

        public void StartGameServer(Game game)
        {
            // Create a new ProcessStartInfo object and set the necessary properties
            ProcessStartInfo serverProcessInfo = new ProcessStartInfo(Path.Combine(GameServerLocation, game.GameLocation, game.GameExeLocation))
            {
                UseShellExecute = false, // Set to false to redirect standard input, output, and error
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                WorkingDirectory = Path.Combine(GameServerLocation, game.GameLocation),
                CreateNoWindow = true // Set to true to hide the cmd window
            };

            if (game.ServerConfiguration is not null)
            {
                UpdateConfigurationFile(game);
            }

            string? runTimeArguments = game.ServerConfiguration.RunArguments;

            if (runTimeArguments is not null)
            {
                if (runTimeArguments.Contains("REPLACEWITHPATH"))
                {
                    runTimeArguments = runTimeArguments.Replace("REPLACEWITHPATH", Path.Combine(GameServerLocation, game.GameLocation));
                }

                if (runTimeArguments.Contains("WORLDNAME"))
                {
                    runTimeArguments = runTimeArguments.Replace("WORLDNAME", game.WorldName);
                }
            }

            serverProcessInfo.Arguments = runTimeArguments;

            // Start the process
            _gameServerProcess = new Process();
            _gameServerProcess.StartInfo = serverProcessInfo;
            _gameServerProcess.OutputDataReceived += ServerProcess_OutputDataReceived;
            _gameServerProcess.ErrorDataReceived += ServerProcess_ErrorDataReceived;
            _gameServerProcess.Start();

            _gameServerProcess.BeginOutputReadLine();
            _gameServerProcess.BeginErrorReadLine();
        }

        private void UpdateConfigurationFile(Game game)
        {
            StringBuilder stringBuilder = new StringBuilder();

            foreach (var configuration in game.ServerConfiguration.Entries)
            {
                string? content = configuration.Value.Content;

                if (content is not null && content.Contains("REPLACEWITHPATH"))
                {
                    content = content.Replace("REPLACEWITHPATH", Path.Combine(GameServerLocation, game.GameLocation));

                    Directory.CreateDirectory(Path.GetDirectoryName(content));
                }

                if (configuration.Value.IsEnabled)
                {
                    stringBuilder.Append($"{configuration.Key}{game.ServerConfiguration.KeyValueSplitter}{content}\n");
                }
                else
                {
                    stringBuilder.Append($"{game.ServerConfiguration.DisabledString}{configuration.Key}{game.ServerConfiguration.KeyValueSplitter}{content}\n");
                }
            }

            string configurationFile = stringBuilder.ToString();

            try
            {
                // Open the file for writing
                using StreamWriter writer = new StreamWriter(Path.Combine(GameServerLocation, game.GameLocation, game.ServerConfiguration.FileLocation));

                // Write the content to the file
                writer.WriteLine(configurationFile.ToString());
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: " + ex.Message);
            }
        }

        public async Task SendInputToGameServer(string command)
        {
            SendHubMessage("USER COMMAND", command);

            if (_gameServerProcess is not null && !string.IsNullOrEmpty(command))
            {
                // Send the command to the standard input of the process
                await _gameServerProcess.StandardInput.WriteLineAsync(command + Environment.NewLine);
            }
            else
            {
                // Handle the case where the process is not running or the command is empty
                SendHubMessage("SERVER ERROR", "Server process is not running, or the command is empty.");
            }
        }

        public void StopGameServer(Game game)
        {
            if (_gameServerProcess is not null)
            {
                if (game.ServerConfiguration.ExitArgument == null)
                {
                    _gameServerProcess.Kill();
                }
                else
                {
                    SendCommand(_gameServerProcess, game.ServerConfiguration.ExitArgument);
                    _gameServerProcess.Close();
                }

                _gameServerProcess = null;
            }
        }

        private void SteamCmdProcess_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.Data))
            {
                _chatHubContext.Clients.All.SendAsync("ReceiveMessage", "STEAMCMD", HideSensitivePhrase(e.Data, SteamLogin.Username));

                if (e.Data.Contains($"Success! App '{_gameId}' already up to date."))
                {
                    _steamCmdInputAllowedEvent.Set();
                }
            }
        }

        private void SteamCmdProcess_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.Data))
            {
                SendHubMessage("STEAMCMD ERROR", HideSensitivePhrase(e.Data, SteamLogin.Username));
            }
        }

        private void ServerProcess_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.Data))
            {
                SendHubMessage("SERVER", HideSensitivePhrase(e.Data, SteamLogin.Username));
            }
        }

        private void ServerProcess_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.Data))
            {
                SendHubMessage("SERVER ERROR", HideSensitivePhrase(e.Data, SteamLogin.Username));
            }
        }

        private static string HideSensitivePhrase(string input, string? sensitivePhrase)
        {
            if (sensitivePhrase is null)
            {
                return input;
            }

            return input.Replace(sensitivePhrase, "*****");
        }

        private readonly Queue<(string, string)> messageQueue = new Queue<(string, string)>();

        private void OnBatchTimerElapsed(object sender, ElapsedEventArgs e)
        {
            (string, string)[] messagesToBatch;

            lock (messageQueue)
            {
                // Dequeue all messages in the queue to batch them
                messagesToBatch = messageQueue.ToArray();
                messageQueue.Clear();
            }

            if (messagesToBatch.Length > 0)
            {
                // Batch the messages into a single message
                string batchedMessage = string.Join("\n", messagesToBatch);

                // Send the batched message (you'll need your SendMessageAsync() logic here)
                _chatHubContext.Clients.All.SendAsync("ReceiveMessage", "SERVER", batchedMessage);
            }
        }

        private void SendHubMessage(string category, string message)
        {
            lock (messageQueue)
            {
                messageQueue.Enqueue((string.Empty, $"{category} : {message}"));
            }
        }

        public async void SendCommand(Process process, string command)
        {
            if (process is not null && !string.IsNullOrEmpty(command))
            {
                // Send the command to the standard input of the process
                process.StandardInput.WriteLine(command);
                process.StandardInput.WriteLine(command);
                process.StandardInput.WriteLine(command);
                process.StandardInput.WriteLine(command);
            }
            else
            {
                // Handle the case where the process is not running or the command is empty
                SendHubMessage("ERROR", "Process is not running, or the command is empty.");
            }
        }

        public void Dispose()
        {
            _steamCmdProcess?.Kill();
            _gameServerProcess?.Kill();
        }
    }
}