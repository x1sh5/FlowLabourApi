using FlowLabourApi.Config;
using FlowLabourApi.Models;
using FlowLabourApi.Models.context;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
namespace FlowLabourApi.Hubs;

////[Authorize]
//public class ChatHub : Hub<FlowHubCallerClients>
//{
//    public async Task SendMessage(string user, string message)
//    {
//        Clients.All.SendAsync("ReceiveMessage", user, message);
//    }
//}

[Authorize]
public class ChatHub : Hub
{
    private readonly XiangxpContext _context;

    public ChatHub(XiangxpContext context)
    {
        _context = context;
    }
    
    public async Task SendMessage(string user, string message)
    {
        Clients.All.SendAsync("ReceiveMessage", user, message);
    }

    public async Task SendToUser(string user, Message message)
    {
        var id = Context.User.Claims.FirstOrDefault(User => User.Type == JwtClaimTypes.IdClaim).Value;
        Clients.User(user).SendAsync("ReceiveMessage", user, message.Content);
    }
}