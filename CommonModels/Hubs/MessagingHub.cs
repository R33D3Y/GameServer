using CommonModels.Services;
using Microsoft.AspNetCore.SignalR;

namespace CommonModels.Hubs
{
    public class MessagingHub : Hub
    {
        private readonly GameService _gameService;

        public MessagingHub(GameService gameService)
        {
            _gameService = gameService;
        }

        public async Task SendToClient(string content, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", content, message);
        }

        public async Task SendToServer(string command, string extra)
        {
            await _gameService.SendInputToGameServer(command);
        }
    }
}