using FlowLabourApi.Models;
using Microsoft.AspNetCore.Identity;

namespace FlowLabourApi.Authentication
{
    public class FlowRoleValidator : IRoleValidator<Role>
    {
        /// <inheritdoc/>
        public async Task<IdentityResult> ValidateAsync(RoleManager<Role> manager, Role role)
        {
            var role1 = await manager.FindByIdAsync(role.Roleid.ToString());
            if (role1 == null)
            {
                return IdentityResult.Failed(new IdentityError
                {
                    Code = "RoleNotExist",
                    Description = $"角色{role.Roleid}不存在"
                });
            }

            return IdentityResult.Success;
        }
    }
}
