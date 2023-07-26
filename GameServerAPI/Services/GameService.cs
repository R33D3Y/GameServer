namespace GameServerAPI.Services
{
    using System.Diagnostics;

    public class GameService
    {
        private const string CMDProcess = "cmd.exe";
        private const string SteamProcess = "steamcmd";
        private const string SteamLoginPrompt = "+login";
        private const string SteamForceInstall = "+force_install_dir";
        private const string SteamAppUpdate = "+app_update";
        private const string TerrariaLocation = "Terraria";
        private const string TerrariaId = "105600";

        private string GameServerLocation;

        private Process? _steamCmdProcess;
        private Process? _serverProcess;

        private AutoResetEvent _steamCmdInputAllowedEvent = new AutoResetEvent(false);

        public GameService(string serverLocation)
        {
            GameServerLocation = serverLocation;
        }

        public void StartAndUpdateSteamCMD(string gameFolderLocation = TerrariaLocation, string gameId = TerrariaId)
        {
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

            SendCommand(_steamCmdProcess, $"{SteamProcess} {SteamForceInstall} {Path.Combine(GameServerLocation, gameFolderLocation)} {SteamLoginPrompt} {SteamLogin.Username} {SteamAppUpdate} {gameId}");

            _steamCmdInputAllowedEvent.WaitOne();
            _steamCmdProcess.Close();
        }

        public void StartGameServer(string gameFolderLocation, string gameExeLocation)
        {
            // Create a new ProcessStartInfo object and set the necessary properties
            ProcessStartInfo serverProcessInfo = new ProcessStartInfo(Path.Combine(GameServerLocation, gameFolderLocation, gameExeLocation))
            {
                UseShellExecute = false, // Set to false to redirect standard input, output, and error
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true // Set to true to hide the cmd window
            };

            // Start the process
            _serverProcess = new Process();
            _serverProcess.StartInfo = serverProcessInfo;
            _serverProcess.OutputDataReceived += ServerProcess_OutputDataReceived;
            _serverProcess.ErrorDataReceived += ServerProcess_ErrorDataReceived;
            _serverProcess.Start();

            _serverProcess.BeginOutputReadLine();
            _serverProcess.BeginErrorReadLine();
        }

        private void SteamCmdProcess_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.Data))
            {
                Debug.WriteLine($"STEAMCMD: {e.Data}");

                if (e.Data.Contains($"Success! App '{TerrariaId}' already up to date."))
                {
                    _steamCmdInputAllowedEvent.Set();
                }
            }
        }

        private void SteamCmdProcess_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.Data))
            {
                Debug.WriteLine($"STEAMCMD ERROR: {e.Data}");
            }
        }

        private void ServerProcess_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.Data))
            {
                Debug.WriteLine($"SERVER: {e.Data}");
            }
        }

        private void ServerProcess_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.Data))
            {
                Debug.WriteLine($"SERVER ERROR: {e.Data}");
            }
        }

        public void SendCommand(Process process, string command)
        {
            if (process is not null && !string.IsNullOrEmpty(command))
            {
                // Send the command to the standard input of the process
                process.StandardInput.WriteLine(command);
            }
            else
            {
                // Handle the case where the process is not running or the command is empty
                Console.WriteLine("SteamCMD process is not running, or the command is empty.");
            }
        }
    }
}