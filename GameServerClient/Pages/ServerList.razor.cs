namespace GameServerClient.Pages
{
    using CommonModels;
    using Microsoft.AspNetCore.Components.Web;
    using Microsoft.AspNetCore.SignalR.Client;
    using Microsoft.JSInterop;
    using System.Text;
    using System.Text.Json;

    public partial class ServerList
    {
        private Game[]? games;
        private const string GameRoute = "game";
        private string? commandInput;
        private HubConnection? hubConnection;

        protected override async Task OnInitializedAsync()
        {
            await UpdateGames();

            hubConnection = new HubConnectionBuilder()
                .WithUrl("https://localhost:6001/chathub")
                .Build();

            hubConnection.On<string, string>("ReceiveMessage", async (user, message) =>
            {
                var encodedMsg = $"{user}: {message}";
                await AddTimelineItem(encodedMsg);
                await InvokeAsync(StateHasChanged);
            });

            await hubConnection.StartAsync();
        }

        private async Task UpdateGames()
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
        }

        private readonly List<string> messages = new List<string>();

        public bool IsConnected => hubConnection?.State == HubConnectionState.Connected;

        public async ValueTask DisposeAsync()
        {
            if (hubConnection is not null)
            {
                await hubConnection.DisposeAsync();
            }
        }

        private async Task SendCommandClick()
        {
            if (hubConnection is not null)
            {
                await SendCommand();
            }
        }

        private async Task HandleKeyPress(KeyboardEventArgs e)
        {
            if (e.Key == "Enter")
            {
                await SendCommand();
            }
        }

        private async Task SendCommand()
        {
            if (hubConnection is not null && !string.IsNullOrEmpty(commandInput))
            {
                commandInput = "SEND IT";
                await hubConnection.SendAsync("SendMessage", commandInput);
            }
        }

        private async Task InstallGameServerClick(Game game)
        {
            switch (game.Name)
            {
                case "Terraria":
                    await InstallServer(game);
                    break;
                case "7 Days To Die":
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

            await UpdateGames();
        }

        private async Task StopServerClick()
        {
            await HttpClient.GetAsync(Route(GameRoute, "StopServer"));

            await UpdateGames();
        }

        private async Task StartServer(Game game)
        {
            string jsonPayload = JsonSerializer.Serialize(game);

            await HttpClient.PostAsync(Route(GameRoute, "StartServer"), new StringContent(jsonPayload, Encoding.UTF8, "application/json"));

            await UpdateGames();
        }

        private static string Route(string route, string endpoint)
        {
            return $"{route}/{endpoint}";
        }

        private async Task AddTimelineItem(string message)
        {
            // Add the new item to the 'messages' collection (assuming 'messages' is your list of timeline items)
            messages.Add(message);
            StateHasChanged(); // Notify Blazor to re-render the component with the new item

            // Scroll to the bottom after the new item is added and the component is re-rendered
            await JSRuntime.InvokeVoidAsync("scrollToBottom");
        }
    }
}