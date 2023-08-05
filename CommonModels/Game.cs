namespace CommonModels
{
    public class Game
    {
        public string Name { get; set; }

        public bool IsSteam { get; set; }

        public bool IsInstalled { get; set; }

        public bool IsRunning { get; set; }

        public string GameLocation { get; set; }

        public string GameExeLocation { get; set; }

        public string? GameId { get; set; }

        public List<string> ServerConfiguration { get; set; }
    }
}