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
                IsWorldInConfigs = true,
                GameExeLocation = string.Empty,
            },
            new Game
            {
                Name = "7 Days To Die",
                IsSteam = true,
                GameLocation = "7Days2Die",
                GameExeLocation = "7DaysToDieServer.exe",
                GameId = "294420",
                IsWorldInConfigs = true,
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
                IsWorldInConfigs = true,
                ServerConfigurationLocation = "serverconfig.txt",
                ServerRunConfiguration = $"-config \"serverconfig.txt\"",
                ServerConfiguration = new Dictionary<string, ServerConfigEntry>
                {
                    { "world", new ServerConfigEntry(true, "REPLACEWITHPATH\\Worlds\\world1.wld")},
                    { "autocreate", new ServerConfigEntry(true, "1")},
                    { "seed", new ServerConfigEntry(true, "AwesomeSeed")},
                    { "worldname", new ServerConfigEntry(true, "world1")},
                    { "difficulty", new ServerConfigEntry(true, "0")},
                    { "maxplayers", new ServerConfigEntry(false, "8")},
                    { "password", new ServerConfigEntry(true, "password")},
                    { "motd", new ServerConfigEntry(true, "Please don't cut the purple trees!")},
                },
                ServerExitArgument = "Exit",
                ServerConfigurationDisabled = "#",
                ServerConfigurationKeyValueSplitter = "=",
            },
            new Game
            {
                Name = "Conan Exiles",
                IsSteam = true,
                GameLocation = "ConanExiles",
                GameExeLocation = "ConanSandboxServer.exe",
                GameId = "443030",
                IsWorldInConfigs = false,
                ServerRunConfiguration = $"-log -userdir=\"REPLACEWITHPATH\\WORLDNAME\"",
            },
            new Game
            {
                Name = "The Forest",
                IsSteam = true,
                GameLocation = "TheForest",
                GameExeLocation = "TheForestDedicatedServer.exe",
                GameId = "556450",
                IsWorldInConfigs = true,
                ServerConfigurationLocation = "Config.cfg",
                ServerRunConfiguration = $"-configfilepath \"REPLACEWITHPATH\\Config.cfg\"",
                ServerConfiguration = new Dictionary<string, ServerConfigEntry>
                {
                    { "serverName", new ServerConfigEntry(true, "R33D3Ys Server")},
                    { "serverPassword", new ServerConfigEntry(true, "password")},
                    { "serverPlayers", new ServerConfigEntry(true, "8")},
                    { "serverAutoSaveInterval", new ServerConfigEntry(true, "10")},
                    { "slot", new ServerConfigEntry(true, "1")},
                    { "showLogs", new ServerConfigEntry(true, "on")},
                    { "saveFolderPath", new ServerConfigEntry(true, "REPLACEWITHPATH\\Worlds\\")},
                    { "initType", new ServerConfigEntry(true, "New")},
                },
                ServerConfigurationDisabled = "//",
                ServerConfigurationKeyValueSplitter = " ",
            },
        };
    }
}
