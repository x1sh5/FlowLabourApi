using FlowLabourApi.Config;
using FlowLabourApi.Models;
using FlowLabourApi.Models.context;
using FlowLabourApi.Models.Services;
using FlowLabourApi.ViewModels;
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
    private readonly static ConnectionMapping<string> _connections =
            new ConnectionMapping<string>();

    private readonly MessageService _messageService;

    public ChatHub(MessageService messageService)
    {
        _messageService = messageService;
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public override async Task OnConnectedAsync()
    {
        var claim = Context.User!.Claims.FirstOrDefault(User => User.Type == JwtClaimTypes.IdClaim);
        if (claim != null)
        {
            var id = claim.Value;
            _connections.Add(id, Context.ConnectionId);
            await base.OnConnectedAsync();
        }
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var claim = Context.User!.Claims.FirstOrDefault(User => User.Type == JwtClaimTypes.IdClaim);
        if (claim != null)
        {
            var id = claim.Value;
            _connections.Remove(id, Context.ConnectionId);
            await base.OnDisconnectedAsync(exception);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="datas"></param>
    /// <returns></returns>
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

    /// <summary>
    /// 
    /// </summary>
    /// <param name="user"></param>
    /// <param name="message"></param>
    /// <returns></returns>
    public async Task SendToUser(string user, MessageView messageView)
    {
        var connectionId = Context.ConnectionId;
        var claim = Context.User!.Claims.FirstOrDefault(User => User.Type == JwtClaimTypes.IdClaim);
        if (claim != null)
        {
            var id = claim.Value;
            messageView.From = int.Parse(id);
            int to = messageView.To;
            var connectionIds = _connections.GetConnections(to.ToString());
            await _messageService.Add(messageView);
            await Clients.Client(connectionId).SendAsync("ReceiveMessage", user, messageView.Content);
        }

    }
}