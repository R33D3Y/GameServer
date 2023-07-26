using CommonModels;
using GameServerAPI.Services;
using Microsoft.AspNetCore.Mvc;

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
            _gameService = new GameService(@"F:\GameServers");
        }

        [HttpGet(nameof(GetGames))]
        public IEnumerable<Game> GetGames()
        {
            return new List<Game>
            {
                new Game("Minecraft"),
                new Game("7DaysToDie"),
                new Game("Terraria"),
                new Game("ConanExiles"),
            }
            .ToArray();
        }

        [HttpGet(nameof(StartSteamCMD))]
        public IActionResult StartSteamCMD(
            string gameLocation,
            string gameId,
            string gameExeLocation)
        {
            _gameService.StartAndUpdateSteamCMD(gameLocation, gameId);
            _gameService.StartGameServer(gameLocation, gameExeLocation);

            return Ok("StartSteamCMD executed successfully!");
        }
    }
}