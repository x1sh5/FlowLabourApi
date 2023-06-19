using FlowLabourApi.Models;
using Microsoft.AspNetCore.Identity;

namespace FlowLabourApi.Authentication
{
    public class FlowRoleValidator : IRoleValidator<Role>
    {
        public Task<IdentityResult> ValidateAsync(RoleManager<Role> manager, Role role)
        {
            throw new NotImplementedException();
        }
    }
}
