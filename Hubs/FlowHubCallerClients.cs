using FlowLabourApi.Models;
using Microsoft.AspNetCore.SignalR;

namespace FlowLabourApi.Hubs
{
    public class FlowHubCallerClients : IHubCallerClients<AuthUser>
    {
        public AuthUser Caller => throw new NotImplementedException();

        public AuthUser Others => throw new NotImplementedException();

        public AuthUser All => throw new NotImplementedException();

        public AuthUser AllExcept(IReadOnlyList<string> excludedConnectionIds)
        {
            throw new NotImplementedException();
        }

        public AuthUser Client(string connectionId)
        {
            throw new NotImplementedException();
        }

        public AuthUser Clients(IReadOnlyList<string> connectionIds)
        {
            throw new NotImplementedException();
        }

        public AuthUser Group(string groupName)
        {
            throw new NotImplementedException();
        }

        public AuthUser GroupExcept(string groupName, IReadOnlyList<string> excludedConnectionIds)
        {
            throw new NotImplementedException();
        }

        public AuthUser Groups(IReadOnlyList<string> groupNames)
        {
            throw new NotImplementedException();
        }

        public AuthUser OthersInGroup(string groupName)
        {
            throw new NotImplementedException();
        }

        public AuthUser User(string userId)
        {
            throw new NotImplementedException();
        }

        public AuthUser Users(IReadOnlyList<string> userIds)
        {
            throw new NotImplementedException();
        }
    }
}
