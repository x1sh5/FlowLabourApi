using FlowLabourApi.Config;
using Microsoft.AspNetCore.SignalR;

namespace FlowLabourApi.Hubs
{
    public class FlowUserIdProvider : IUserIdProvider
    {
        public string? GetUserId(HubConnectionContext connection)
        {
            var user = connection.User.Claims.FirstOrDefault(x => x.Type == JwtClaimTypes.IdClaim);
            if (user == null)
            {

                return "anony-" + Guid.NewGuid().ToString("N");
            }
            return user.Value;
        }
    }
}
