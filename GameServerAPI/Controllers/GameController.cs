using Microsoft.AspNetCore.Mvc;

namespace GameServerAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class GameController : ControllerBase
    {
        private readonly ILogger<GameController> _logger;

        public GameController(ILogger<GameController> logger)
        {
            _logger = logger;
        }

        [HttpGet(Name = "GetGames")]
        public IEnumerable<Game> Get()
        {
            return new List<Game>
            {
                new Game("Minecraft"),
                new Game("7DaysToDie"),
                new Game("ConanExiles"),
            }
            .ToArray();
        }
    }

    public class Game
    {
        public string Name { get; set; }

        public Game(string name)
        {
            Name = name;
        }
    }
}