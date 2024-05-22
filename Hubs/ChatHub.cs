using Microsoft.AspNetCore.SignalR;
using Sahaj_Yatri.Models.Dto;
using Sahaj_Yatri.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sahaj_Yatri.Hubs
{
    public class ChatHub : Hub
    {
       
        public async Task SendMessage(string message)
        {
            await Clients.All.SendAsync("ReceivedMessage", message);
        }

    }
}