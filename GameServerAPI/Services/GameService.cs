using System.Diagnostics;

namespace GameServerAPI.Services
{
    public class GameService
    {
        private Process? _steamCmdProcess;
        private const string CMDProcess = "cmd.exe";
        private const string SteamProcess = "steamcmd +login anonymous";
        private const string SteamLogin = "steamcmd +login anonymous";
        private const string SteamForceInstall = "+force_install_dir";
        private const string SteamAppUpdate = "+app_update";
        private const string GameServers = @"F:\GameServers";

        private const string TerrariaLocation = "Terraria";
        private const string TerrariaId = "105610";

        public async void StartSteamCMD()
        {
            // Create a new ProcessStartInfo object and set the necessary properties
            ProcessStartInfo steamCmdProcessInfo = new ProcessStartInfo(CMDProcess);
            steamCmdProcessInfo.UseShellExecute = false; // Set to false to redirect standard input, output, and error
            steamCmdProcessInfo.RedirectStandardInput = true;
            steamCmdProcessInfo.RedirectStandardOutput = true;
            steamCmdProcessInfo.RedirectStandardError = true;
            steamCmdProcessInfo.CreateNoWindow = true; // Set to true to hide the cmd window

            // Start the process
            _steamCmdProcess = new Process();
            _steamCmdProcess.StartInfo = steamCmdProcessInfo;
            _steamCmdProcess.OutputDataReceived += SteamCmdProcess_OutputDataReceived;
            _steamCmdProcess.ErrorDataReceived += SteamCmdProcess_ErrorDataReceived;
            _steamCmdProcess.Start();

            _steamCmdProcess.BeginOutputReadLine();
            _steamCmdProcess.BeginErrorReadLine();

            SendCommand($"{SteamProcess} {SteamLogin} {SteamForceInstall} {Path.Combine(GameServers, TerrariaLocation)} {SteamAppUpdate} {TerrariaId}");
        }

        private void SteamCmdProcess_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.Data))
            {
                Debug.WriteLine(e.Data);
            }
        }

        private void SteamCmdProcess_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.Data))
            {
                Debug.WriteLine($"Error: {e.Data}");
            }
        }

        public void SendCommand(string command)
        {
            if (_steamCmdProcess is not null && !string.IsNullOrEmpty(command))
            {
                // Send the command to the standard input of the process
                _steamCmdProcess.StandardInput.WriteLine(command);
            }
            else
            {
                // Handle the case where the process is not running or the command is empty
                Console.WriteLine("SteamCMD process is not running, or the command is empty.");
            }
        }

        public void StopSteamCMD()
        {
            if (_steamCmdProcess is not null)
            {
                _steamCmdProcess.Kill();
            }
        }
    }
}