using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using SignalRChat.Models.Chat;
using System.Threading.Tasks;

namespace SignalRChat.Hubs
{
    //[Authorize]
    public class ChatHub : Hub
    {
        public async Task Send(string message)
        {
            await this.Clients.All.SendAsync("NewMessage", new Message()
            {
                User = this.Context.User.Identity.Name,
                Text = message
            });
        }
    }
}

