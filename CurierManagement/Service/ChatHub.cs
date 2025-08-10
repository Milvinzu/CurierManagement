using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CurierManagement.Service
{
    public class ChatHub : Hub
    {
        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message, DateTime.Now.ToString("HH:mm:ss"));
        }

        public async Task JoinChat(string user)
        {
            await Clients.All.SendAsync("UserJoined", user, DateTime.Now.ToString("HH:mm:ss"));
        }

        public async Task LeaveChat(string user)
        {
            await Clients.All.SendAsync("UserLeft", user, DateTime.Now.ToString("HH:mm:ss"));
        }
    }
}
