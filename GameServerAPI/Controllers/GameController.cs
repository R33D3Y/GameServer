using CommonModels;
using CommonModels.Services;
using Microsoft.AspNetCore.Mvc;

namespace GameServerAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class GameController : ControllerBase
    {
        private readonly ILogger<GameController> _logger;
        private readonly GameService _gameService;

        public GameController(ILogger<GameController> logger, GameService gameService)
        {
            _logger = logger;
            _gameService = gameService;
        }

        [HttpGet(nameof(GetGames))]
        public IEnumerable<Game> GetGames()
        {
            var games = new List<Game>();

            foreach (var game in Games.AvailableGames)
            {
                game.IsInstalled = Directory.Exists(Path.Combine(_gameService.GameServerLocation, game.GameLocation));
                game.IsRunning = _gameService.CurrentlyRunningGame?.Equals(game.Name) ?? false;

                if (game.ServerConfigurationLocation is not null)
                {
                    string configurationPath = Path.Combine(_gameService.GameServerLocation, game.GameLocation, game.ServerConfigurationLocation);
                    if (System.IO.File.Exists(configurationPath))
                    {
                        try
                        {
                            // Read all lines from the file and store them in an array
                            string[] lines = System.IO.File.ReadAllLines(configurationPath);

                            // Process each line
                            foreach (string line in lines)
                            {
                                if (string.IsNullOrEmpty(line))
                                {
                                    continue;
                                }

                                bool isenabled = !line.StartsWith('#');
                                string[] splitLine = line.Replace("#", "").Split('=');
                                ServerConfigEntry serverConfigEntry = isenabled ? new ServerConfigEntry(true, splitLine[1]) : new ServerConfigEntry(false, splitLine[1]);

                                if (game.ServerConfiguration.ContainsKey(splitLine[0]))
                                {
                                    game.ServerConfiguration[splitLine[0]] = serverConfigEntry;
                                }
                                else
                                {
                                    game.ServerConfiguration.Add(splitLine[0], serverConfigEntry);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("An error occurred: " + ex.Message);
                        }
                    }
                }

                games.Add(game);
            }

            return games;
        }

        [HttpPost(nameof(InstallServer))]
        public IActionResult InstallServer(
            [FromBody] Game game)
        {
            _gameService.StartAndUpdateSteamCMD(game);

            return Ok($"Installed {game.Name} successfully!");
        }

        [HttpPost(nameof(StartServer))]
        public IActionResult StartServer(
            [FromBody] Game game)
        {
            _gameService.CurrentlyRunningGame = game.Name;

            _gameService.StartAndUpdateSteamCMD(game);
            _gameService.StartGameServer(game);

            return Ok($"Started {game.Name} successfully!");
        }

        [HttpPost(nameof(SendInputCommand))]
        public async Task<IActionResult> SendInputCommand(
            [FromBody] string inputCommand)
        {
            if (!string.IsNullOrEmpty(inputCommand))
            {
                await _gameService.SendInputToGameServer(inputCommand);
            }

            return Ok($"Sent command '{inputCommand}' successfully!");
        }

        [HttpPost(nameof(StopServer))]
        public IActionResult StopServer(
            [FromBody] Game game)
        {
            _gameService.CurrentlyRunningGame = null;
            _gameService.StopGameServer(game);

            return Ok($"Stopped game server successfully!");
        }
    }
}