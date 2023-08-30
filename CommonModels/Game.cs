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

        public string? ServerRunConfiguration { get; set; }

        public string? ServerConfigurationLocation { get; set; }

        public Dictionary<string, ServerConfigEntry>? ServerConfiguration { get; set; }

        public string? ServerExitArgument { get; set; }
    }

    public class ServerConfigEntry
    {
        public ServerConfigEntry(bool isEnabled, string content)
        {
            IsEnabled = isEnabled;
            Content = content;
        }

        public bool IsEnabled { get; set; }

        public string Content { get; set; }
    }
}