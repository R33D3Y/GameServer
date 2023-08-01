using Microsoft.AspNetCore.SignalR;

namespace CommonModels.Hubs
{
    public class MessagingHub : Hub
    {
        public async Task SendMessage(string content)
        {
            await Clients.All.SendAsync("ReceiveMessage", content);
        }
    }
}