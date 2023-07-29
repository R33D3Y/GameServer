namespace GameServerClient.Pages
{
    using CommonModels;
    using Microsoft.AspNetCore.SignalR.Client;
    using System.Text;
    using System.Text.Json;

    public partial class ServerList
    {
        private Game[]? games;
        private const string GameRoute = "game";

        protected override async Task OnInitializedAsync()
        {
            var response = await HttpClient.GetAsync(Route(GameRoute, "GetGames"));
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();

                if (json is not null)
                {
                    games = JsonSerializer.Deserialize<Game[]?>(json, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                }
            }
            else
            {
                // Handle the error case if the response is not successful
                // You can display an error message or handle it as per your application's requirements
            }

            hubConnection = new HubConnectionBuilder()
                .WithUrl("https://localhost:6001/chathub")
                .Build();

            hubConnection.On<string, string>("ReceiveMessage", (user, message) =>
            {
                var encodedMsg = $"{user}: {message}";
                messages.Add(encodedMsg);
                InvokeAsync(StateHasChanged);
            });

            await hubConnection.StartAsync();
        }

        private HubConnection? hubConnection;
        private readonly List<string> messages = new List<string>();

        public bool IsConnected => hubConnection?.State == HubConnectionState.Connected;

        public async ValueTask DisposeAsync()
        {
            if (hubConnection is not null)
            {
                await hubConnection.DisposeAsync();
            }
        }

        private async Task InstallGameServerClick(Game game)
        {
            switch (game.Name)
            {
                case "Terraria":
                    await InstallServer(game);
                    break;
                default:
                    throw new ArgumentException("Game not found");
            }
        }

        private async Task StartGameServerClick(Game game)
        {
            switch (game.Name)
            {
                case "Terraria":
                    await StartServer(game);
                    break;
                default:
                    throw new ArgumentException("Game not found");
            }
        }

        private async Task InstallServer(Game game)
        {
            string jsonPayload = JsonSerializer.Serialize(game);

            await HttpClient.PostAsync(Route(GameRoute, "InstallServer"), new StringContent(jsonPayload, Encoding.UTF8, "application/json"));
        }

        private async Task StopServerClick()
        {
            await HttpClient.GetAsync(Route(GameRoute, "StopServer"));
        }

        private async Task StartServer(Game game)
        {
            string jsonPayload = JsonSerializer.Serialize(game);

            await HttpClient.PostAsync(Route(GameRoute, "StartServer"), new StringContent(jsonPayload, Encoding.UTF8, "application/json"));
        }

        private static string Route(string route, string endpoint)
        {
            return $"{route}/{endpoint}";
        }
    }
}