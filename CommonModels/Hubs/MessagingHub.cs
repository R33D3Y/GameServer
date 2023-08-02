using Microsoft.AspNetCore.SignalR;

namespace CommonModels.Hubs
{
    public class MessagingHub : Hub
    {
        public async Task SendToClient(string content, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", content, message);
        }
    }
}