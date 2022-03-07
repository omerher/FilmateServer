using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace FilmateAPI.Hubs
{
    public class ChatHub : Hub
    {
        public async Task SendMessage(string userId, string message)
        {
            await Clients.Others.SendAsync("ReceiveMessage", userId, message);
        }
    }
}
