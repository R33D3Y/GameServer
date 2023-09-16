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

#if DEBUG
            hubConnection = new HubConnectionBuilder()
                .WithUrl($"{Settings.LocalHost}/chathub")
                .Build();
#else
            hubConnection = new HubConnectionBuilder()
                .WithUrl($"{Settings.RemoteHost}/chathub")
                .Build();
#endif

            hubConnection.On<string, string>("ReceiveMessage", async (_, message) =>
            {
                string[] splitMessage = message.Split('\n');

                foreach (string line in splitMessage)
                {
                    string tempString = line.Split(',')[1];
                    tempString = tempString[..^1];

                    await AddTimelineItem(tempString);
                }

                await InvokeAsync(StateHasChanged);
            });

            await hubConnection.StartAsync();
        }

        private async Task SendCommand()
        {
            if (!string.IsNullOrEmpty(commandInput))
            {
                var jsonContent = new StringContent($"\"{commandInput}\"", Encoding.UTF8, "application/json");
                await HttpClient.PostAsync(Route(GameRoute, "SendInputCommand"), jsonContent);

                commandInput = null;
            }
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
            await SendCommand();
        }

        private async Task HandleKeyPress(KeyboardEventArgs e)
        {
            if (e.Key == "Enter")
            {
                await SendCommand();
            }
        }

        private async Task InstallServerClick(Game game)
        {
            if (game.IsSteam)
            {
                await Post(game, "InstallServer");
            }
        }

        private async Task StartServerClick(Game game)
        {
            if (game.IsInstalled)
            {
                await Post(game, "StartServer");
            }
        }

        private async Task StopServerClick(Game game)
        {
            await Post(game, "StopServer");
        }

        private async Task Post(Game game, string endpoint)
        {
            string jsonPayload = JsonSerializer.Serialize(game);

            await HttpClient.PostAsync(Route(GameRoute, endpoint), new StringContent(jsonPayload, Encoding.UTF8, "application/json"));

            await UpdateGames();
        }

        private static string Route(string route, string endpoint)
        {
            return $"{route}/{endpoint}";
        }

        private async Task AddTimelineItem(string message)
        {
            if (messages.Count > 100)
            {
                messages.RemoveAt(0);
            }

            // Add the new item to the 'messages' collection (assuming 'messages' is your list of timeline items)
            messages.Add(message);

            // Scroll to the bottom after the new item is added and the component is re-rendered
            await JSRuntime.InvokeVoidAsync("scrollToBottom");
        }
    }
}