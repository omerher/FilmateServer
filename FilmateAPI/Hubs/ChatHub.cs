using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using FilmateBL.Models;
using FilmateAPI.DataTransferObjects;
using System;

namespace FilmateAPI.Hubs
{
    public class ChatHub : Hub
    {
        #region Add connection to the db context using dependency injection
        FilmateContext context;
        public ChatHub(FilmateContext context)
        {
            this.context = context;
        }
        #endregion

        public async Task SendMessage(MsgDTO message)
        {
            Msg msg = new Msg()
            {
                AccountId = message.AccountId,
                ChatId = message.ChatId,
                Content = message.Content,
                SentDate = DateTime.Now
            };
            Msg returnedMsg = context.AddMsg(msg);
            if (returnedMsg != null)
                await Clients.Others.SendAsync("ReceiveMessage", message);
        }
    }
}
