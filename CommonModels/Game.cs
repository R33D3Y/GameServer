namespace CommonModels
{
    public class Game
    {
        public string Name { get; set; }

        public bool IsSteam { get; set; }

        public string GameLocation { get; set; }

        public string GameExeLocation { get; set; }

        public string GameId { get; set; }

        public Game(string name, bool isSteam, string gameLocation, string gameExeLocation, string gameId)
        {
            Name = name;
            IsSteam = isSteam;
            GameLocation = gameLocation;
            GameExeLocation = gameExeLocation;
            GameId = gameId;
        }
    }
}