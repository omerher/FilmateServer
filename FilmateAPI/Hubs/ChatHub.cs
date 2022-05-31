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
        public ChatHub(FilmateContext context) : base()
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

        public async Task SendMessageToGroup(MsgDTO message, string groupId)
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
            {
                IClientProxy proxy = Clients.Group(groupId);
                await proxy.SendAsync("ReceiveMessageFromGroup", message.AccountId, message.Content, groupId);
            }
        }

        public async Task OnConnect(string[] groupIds)
        {
            foreach (string groupId in groupIds)
                await Groups.AddToGroupAsync(Context.ConnectionId, groupId);
            await base.OnConnectedAsync();
        }
    }
}
