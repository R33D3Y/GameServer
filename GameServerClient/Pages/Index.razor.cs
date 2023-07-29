using Microsoft.AspNetCore.SignalR.Client;
using System.Diagnostics;

namespace GameServerClient.Pages
{
    public partial class Index
    {
        //private HubConnection? hubConnection;
        //private List<string> messages = new List<string>();
        //private string? userInput = "Test1";
        //private string? messageInput = "Test2";

        //protected override async Task OnInitializedAsync()
        //{
        //    hubConnection = new HubConnectionBuilder()
        //        .WithUrl("https://localhost:6001/chathub")
        //        .Build();

        //    hubConnection.On<string, string>("ReceiveMessage", (user, message) =>
        //    {
        //        var encodedMsg = $"{user}: {message}";
        //        messages.Add(encodedMsg);
        //        InvokeAsync(StateHasChanged);
        //    });

        //    await hubConnection.StartAsync();
        //}

        //private async Task Send()
        //{
        //    if (hubConnection is not null)
        //    {
        //        await hubConnection.SendAsync("SendMessage", userInput, messageInput);
        //    }
        //}

        //public bool IsConnected => hubConnection?.State == HubConnectionState.Connected;

        //public async ValueTask DisposeAsync()
        //{
        //    if (hubConnection is not null)
        //    {
        //        await hubConnection.DisposeAsync();
        //    }
        //}
    }
}