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
    
    public async Task SendMessage(IList<object> datas)
    {
        int args = datas.Count;
        switch (args) {
            case 1:
                await Clients.All.SendAsync("ReceiveMessage", datas[0]);
                break;
            case 2:
                await Clients.All.SendAsync("ReceiveMessage", datas[0], datas[1]);
                break;
            case 3:
                await Clients.All.SendAsync("ReceiveMessage", datas[0], datas[1], datas[2]);
                break;
            case 4:
                await Clients.All.SendAsync("ReceiveMessage", datas[0], datas[1], datas[2], datas[3]);
                break;
            case 5:
                await Clients.All.SendAsync("ReceiveMessage", datas[0], datas[1], datas[2], datas[3], datas[4]);
                break;
            case 6:
                await Clients.All.SendAsync("ReceiveMessage", datas[0], datas[1], datas[2], datas[3], datas[4], datas[5]);
                break;
            case 7:
                await Clients.All.SendAsync("ReceiveMessage", datas[0], datas[1], datas[2], datas[3], datas[4], datas[5], datas[6]);
                break;
            case 8:
                await Clients.All.SendAsync("ReceiveMessage", datas[0], datas[1], datas[2], datas[3], datas[4], datas[5], datas[6], datas[7]);
                break;
            case 9:
                await Clients.All.SendAsync("ReceiveMessage", datas[0], datas[1], datas[2], datas[3], datas[4], datas[5], datas[6], datas[7], datas[8]);
                break;
            case 10:
                await Clients.All.SendAsync("ReceiveMessage", datas[0], datas[1], datas[2], datas[3], datas[4], datas[5], datas[6], datas[7], datas[8], datas[9]);
                break;
        }

        //await Clients.All.SendAsync("ReceiveMessage", user, message);
    }

    public async Task SendToUser(string user, Message message)
    {
        var id = Context.User.Claims.FirstOrDefault(User => User.Type == JwtClaimTypes.IdClaim).Value;
        Clients.User(user).SendAsync("ReceiveMessage", user, message.Content);
    }
}