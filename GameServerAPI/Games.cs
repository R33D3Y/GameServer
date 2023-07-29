﻿using CommonModels;

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
                GameExeLocation = string.Empty,
                GameId = "294420"
            },
            new Game
            {
                Name = "Terraria",
                IsSteam = true,
                GameLocation = "Terraria",
                GameExeLocation = "TerrariaServer.exe",
                GameId = "105600"
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
