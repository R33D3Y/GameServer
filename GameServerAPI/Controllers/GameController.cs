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
}