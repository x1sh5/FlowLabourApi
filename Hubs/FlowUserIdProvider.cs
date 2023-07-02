using FlowLabourApi.Config;
using FlowLabourApi.Models.state;
using Microsoft.AspNetCore.SignalR;

namespace FlowLabourApi.Hubs
{
    public class FlowUserIdProvider : IUserIdProvider
    {
        public string? GetUserId(HubConnectionContext connection)
        {
            var user = connection.User.Claims.FirstOrDefault(x=>x.Type==JwtClaimTypes.IdClaim);
            if(user==null)
            {
                throw new System.Exception("User not found");
            }
            return user.Value;
        }
    }
}
