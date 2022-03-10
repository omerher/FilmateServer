using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using FilmateBL.Models;
using FilmateAPI.DataTransferObjects;

namespace FilmateAPI.Hubs
{
    public class ChatHub : Hub
    {
        public async Task SendMessage(MsgDTO message)
        {
            await Clients.Others.SendAsync("ReceiveMessage", message);
        }
    }
}
