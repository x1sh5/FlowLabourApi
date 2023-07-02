using FlowLabourApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
namespace FlowLabourApi.Hubs;

//[Authorize]
//public class ChatHub : Hub<FlowHubCallerClients>
//{
//    public async Task SendMessage(string user, string message)
//    {
//        Clients.All.SendAsync("ReceiveMessage", user, message);
//    }
//}


public class ChatHub : Hub
{
    public async Task SendMessage(string user, string message)
    {
        Clients.All.SendAsync("ReceiveMessage", user, message);
    }
}