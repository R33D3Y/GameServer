using CommonModels;
using CommonModels.Hubs;
using GameServerAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace GameServerAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class GameController : ControllerBase
    {
        private readonly IHubContext<ChatHub> _chatHub;
        private readonly ILogger<GameController> _logger;
        private GameService _gameService;

        public GameController(ILogger<GameController> logger, GameService gameService)
        {
            _logger = logger;
            _gameService = gameService;
        }

        [HttpGet(nameof(GetGames))]
        public IEnumerable<Game> GetGames()
        {
            return new List<Game>
            {
                new Game("Minecraft", false, string.Empty, string.Empty, string.Empty),
                new Game("7DaysToDie", true, string.Empty, string.Empty, string.Empty),
                new Game("Terraria", true, "Terraria", "TerrariaServer.exe", "105600"),
                new Game("ConanExiles", true, string.Empty, string.Empty, string.Empty),
            }
            .ToArray();
        }

        [HttpPost(nameof(StartSteamCMD))]
        public IActionResult StartSteamCMD(
            [FromBody] Game game)
        {
            _gameService.StartAndUpdateSteamCMD(game.GameLocation, game.GameId);
            //_gameService.StartGameServer(game.GameLocation, game.GameExeLocation);

            return Ok("StartSteamCMD executed successfully!");
        }
    }
}