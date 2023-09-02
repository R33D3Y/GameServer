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

        public string? WorldName { get; set; }

        public ServerConfiguration? ServerConfiguration { get; set; }
    }

    public class ServerConfiguration
    {
        public string? RunArguments { get; set; }

        public string? FileLocation { get; set; }

        public string? KeyValueSplitter { get; set; }

        public string? DisabledString { get; set; }

        public Dictionary<string, ServerConfigEntry>? Entries { get; set; }

        public string? ExitArgument { get; set; }

        public bool IsWorldInConfigs { get; set; }

        public ConfigurationFileType FileType { get; set; }
    }

    public enum ConfigurationFileType
    {
        Text,
        XML,
        INI
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