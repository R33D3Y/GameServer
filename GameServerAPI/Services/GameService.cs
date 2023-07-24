using System.Diagnostics;

namespace GameServerAPI.Services
{
    public class GameService
    {
        private Process? _steamCmdProcess;

        public void StartSteamCMD()
        {
            string steamCmdPath = @"C:\steamcmd\steamcmd.exe";

            // Create a new ProcessStartInfo object and set the necessary properties
            ProcessStartInfo steamCmdProcessInfo = new ProcessStartInfo(steamCmdPath);
            steamCmdProcessInfo.UseShellExecute = false; // Set to false to redirect standard output
            steamCmdProcessInfo.RedirectStandardOutput = true;
            steamCmdProcessInfo.RedirectStandardInput = true;
            steamCmdProcessInfo.CreateNoWindow = true; // Set to true to hide the cmd window

            // Start the process
            _steamCmdProcess = new Process();
            _steamCmdProcess.StartInfo = steamCmdProcessInfo;
            _steamCmdProcess.OutputDataReceived += _steamCmdProcess_OutputDataReceived;
            _steamCmdProcess.BeginOutputReadLine();
            _steamCmdProcess.Start();
        }

        private void _steamCmdProcess_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            Debug.WriteLine(e.Data);
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