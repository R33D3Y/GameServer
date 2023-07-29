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
        private readonly GameService _gameService;

        public GameController(ILogger<GameController> logger, GameService gameService)
        {
            _logger = logger;
            _gameService = gameService;
        }

        [HttpGet(nameof(GetGames))]
        public IEnumerable<Game> GetGames()
        {
            return Games.AvailableGames.ToArray();
        }

        [HttpPost(nameof(InstallServer))]
        public IActionResult InstallServer(
            [FromBody] Game game)
        {
            _gameService.StartAndUpdateSteamCMD(game.GameLocation, game.GameId);

            return Ok("StartSteamCMD executed successfully!");
        }

        [HttpPost(nameof(StartServer))]
        public IActionResult StartServer(
            [FromBody] Game game)
        {
            _gameService.StartGameServer(game.GameLocation, game.GameExeLocation);

            return Ok("StartSteamCMD executed successfully!");
        }

        [HttpGet(nameof(StopServer))]
        public IActionResult StopServer()
        {
            _gameService.StopGameServer();

            return Ok("StartSteamCMD executed successfully!");
        }
    }
}