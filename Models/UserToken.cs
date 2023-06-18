using Microsoft.AspNetCore.Identity;

namespace FlowLabourApi.Models
{
    public class UserToken : IdentityUserToken<int>
    {
        public DateTime ExpireAt { get; internal set; }

        public virtual AuthUser User { get; set; }
    }
}
