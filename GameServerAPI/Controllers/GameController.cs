using CommonModels;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace GameServerAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class GameController : ControllerBase
    {
        private readonly ILogger<GameController> _logger;
        private GameService _gameService;

        public GameController(ILogger<GameController> logger)
        {
            _logger = logger;
            _gameService = new GameService();
        }

        [HttpGet(nameof(GetGames))]
        public IEnumerable<Game> GetGames()
        {
            return new List<Game>
            {
                new Game("Minecraft"),
                new Game("7DaysToDie"),
                new Game("ConanExiles"),
            }
            .ToArray();
        }

        [HttpGet(nameof(StartSteamCMD))]
        public IActionResult StartSteamCMD()
        {
            _gameService.StartSteamCMD();

            return Ok("StartSteamCMD executed successfully!");
        }
    }

    public class GameService
    {
        public void StartSteamCMD()
        {
            string steamCmdPath = @"C:\steamcmd\steamcmd.exe";

            // Create a new ProcessStartInfo object and set the necessary properties
            ProcessStartInfo steamCmdProcessInfo = new ProcessStartInfo(steamCmdPath);
            steamCmdProcessInfo.UseShellExecute = false; // Set to false to redirect standard output
            steamCmdProcessInfo.RedirectStandardOutput = true;
            steamCmdProcessInfo.CreateNoWindow = true; // Set to true to hide the cmd window

            // Start the process
            Process steamCmdProcess = new Process();
            steamCmdProcess.StartInfo = steamCmdProcessInfo;
            steamCmdProcess.Start();

            // Wait for the process to exit and then read the output
            steamCmdProcess.WaitForExit();
            string output = steamCmdProcess.StandardOutput.ReadToEnd();

            // Display the output (optional)
            Console.WriteLine(output);
        }
    }
}