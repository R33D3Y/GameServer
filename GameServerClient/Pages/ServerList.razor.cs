namespace GameServerClient.Pages
{
    using CommonModels;
    using Microsoft.AspNetCore.Components.Web;
    using Microsoft.AspNetCore.SignalR.Client;
    using Microsoft.JSInterop;
    using System.Collections.Concurrent;
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

                // Buffer the message
                messageBuffer.Enqueue(encodedMsg);

                // Start the flushing process if not already started
                if (!isFlushingBuffer)
                {
                    isFlushingBuffer = true;
                    await FlushBuffer();
                }
            });

            await hubConnection.StartAsync();
        }

        private ConcurrentQueue<string> messageBuffer = new ConcurrentQueue<string>();
        private SemaphoreSlim bufferLock = new SemaphoreSlim(1);
        private bool isFlushingBuffer = false;

        private async Task FlushBuffer()
        {
            while (messageBuffer.TryDequeue(out var bufferedMessage))
            {
                await bufferLock.WaitAsync();

                try
                {
                    // Simulate UI update with the buffered message
                    await AddTimelineItem(bufferedMessage);
                    await InvokeAsync(StateHasChanged);

                    // Introduce a delay to control the rate of updates
                    await Task.Delay(50); // Adjust the delay time as needed
                }
                finally
                {
                    bufferLock.Release();
                }
            }

            isFlushingBuffer = false;
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
            // Add the new item to the 'messages' collection (assuming 'messages' is your list of timeline items)
            messages.Add(message);
            StateHasChanged(); // Notify Blazor to re-render the component with the new item

            // Scroll to the bottom after the new item is added and the component is re-rendered
            await JSRuntime.InvokeVoidAsync("scrollToBottom");
        }
    }
}