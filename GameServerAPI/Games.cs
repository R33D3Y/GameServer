using CommonModels;

namespace GameServerAPI
{
    public static class Games
    {
        public static readonly List<Game> AvailableGames = new()
        {
            new Game
            {
                Name = "Minecraft",
                IsSteam = false,
                GameLocation = "Minecraft",
                GameExeLocation = string.Empty,
            },
            new Game
            {
                Name = "7 Days To Die",
                IsSteam = true,
                GameLocation = "7Days2Die",
                GameExeLocation = "7DaysToDieServer.exe",
                GameId = "294420",
                ServerRunConfiguration = $"-quit -batchmode -nographics -configfile=serverconfig.xml -dedicated",
                ServerExitArgument = "shutdown"
            },
            new Game
            {
                Name = "Terraria",
                IsSteam = true,
                GameLocation = "Terraria",
                GameExeLocation = "TerrariaServer.exe",
                GameId = "105600",
                ServerConfigurationLocation = "serverconfig.txt",
                ServerRunConfiguration = $"-config \"serverconfig.txt\"",
                ServerConfiguration = new Dictionary<string, ServerConfigEntry>
                {
                    { "world", new ServerConfigEntry(true, "F:\\GameServers\\Terraria\\Worlds\\world1.wld")},
                    { "autocreate", new ServerConfigEntry(true, "1")},
                    { "seed", new ServerConfigEntry(true, "AwesomeSeed")},
                    { "worldname", new ServerConfigEntry(true, "world1")},
                    { "difficulty", new ServerConfigEntry(true, "0")},
                    { "maxplayers", new ServerConfigEntry(false, "8")},
                    { "password", new ServerConfigEntry(true, "password")},
                    { "motd", new ServerConfigEntry(true, "Please don't cut the purple trees!")},
                },
                ServerExitArgument = "Exit"
            },
            new Game
            {
                Name = "Conan Exiles",
                IsSteam = true,
                GameLocation = "ConanExiles",
                GameExeLocation = string.Empty,
                GameId = "443030"
            },
        };
    }
}
