using FlowLabourApi.Models;
using FlowLabourApi.Models.context;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FlowLabourApi.Authentication
{
    public class FlowRoleStore : IRoleStore<Role>
    {
        private bool disposedValue;
        private readonly FlowContext _context;
        private readonly ILogger<FlowRoleStore> _logger;

        public FlowRoleStore(FlowContext context, ILogger<FlowRoleStore> logger, bool disposedValue = false)
        {
            this.disposedValue = disposedValue;
            _context = context;
            _logger = logger;
        }

        public async Task<IdentityResult> CreateAsync(Role role, CancellationToken cancellationToken)
        {
            Console.Out.WriteLine("FlowRoleStore.CreateAsync run");
            try
            {
                // 在此添加代码以创建新角色
                _ = await _context.Roles.AddAsync(role, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);
                return IdentityResult.Success;
            }
            catch (Exception ex)
            {
                return IdentityResult.Failed(new IdentityError { Description = ex.Message });
            }
        }

        public Task<IdentityResult> DeleteAsync(Role role, CancellationToken cancellationToken)
        {
            if (role == null)
            {
                throw new ArgumentNullException(nameof(role));
            }
            try
            {
                _context.Roles.Remove(role);
                return _context.SaveChangesAsync(cancellationToken).ContinueWith(task =>
                {
                    return IdentityResult.Success;
                });
            }
            catch (Exception ex)
            {
                return Task.FromResult(IdentityResult.Failed(new IdentityError { Description = ex.Message }));
            }


        }

        public async Task<Role> FindByIdAsync(string roleId, CancellationToken cancellationToken)
        {
            Console.Out.WriteLine("FlowRoleStore.FindByIdAsync run");
            int? id;
            id = int.TryParse(roleId, out int result) ? result : (int?)null;
            if (id == null)
            {
                return null;
            }
            var role = await _context.Roles.FindAsync(roleId);
            if (role == null) throw new ArgumentNullException("角色不存在");
            return role;
        }

        public async Task<Role> FindByNameAsync(string normalizedRoleName, CancellationToken cancellationToken)
        {
            var role = await _context.Roles.FirstOrDefaultAsync(r => r.Privilege == normalizedRoleName);
            if (role == null) throw new ArgumentNullException("角色不存在");
            return role;
        }

        public Task<string> GetNormalizedRoleNameAsync(Role role, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public async Task<string> GetRoleIdAsync(Role role, CancellationToken cancellationToken)
        {
            Console.Out.WriteLine("FlowRoleStore.GetRoleIdAsync run");
            if (role == null)
            {
                throw new ArgumentNullException(nameof(role));
            }
            var role1 = await _context.Roles.FirstOrDefaultAsync(r => r.Roleid == role.Roleid);
            if (role1 == null) throw new ArgumentNullException("角色不存在");
            return role1.Roleid.ToString();
        }

        public async Task<string> GetRoleNameAsync(Role role, CancellationToken cancellationToken)
        {
            Console.Out.WriteLine("FlowRoleStore.GetRoleNameAsync run");
            if (role == null)
            {
                throw new ArgumentNullException(nameof(role));
            }
            var role1 = await _context.Roles.FirstOrDefaultAsync(r => r.Privilege == role.Privilege);
            if (role1 == null) throw new ArgumentNullException("角色不存在");
            return role1.Privilege;
        }

        public Task SetNormalizedRoleNameAsync(Role role, string normalizedName, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="role"></param>
        /// <param name="roleName"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public async Task SetRoleNameAsync(Role role, string roleName, CancellationToken cancellationToken)
        {
            if (role == null)
            {
                throw new ArgumentNullException(nameof(role));
            }
            role.Privilege = roleName;
            await UpdateAsync(role, cancellationToken);
        }

        public Task<IdentityResult> UpdateAsync(Role role, CancellationToken cancellationToken)
        {
            if (role == null)
            {
                throw new ArgumentNullException(nameof(role));
            }
            try
            {
                _context.Roles.Update(role);
                return _context.SaveChangesAsync(cancellationToken).ContinueWith(task =>
                {
                    return IdentityResult.Success;
                });
            }
            catch (Exception ex)
            {
                return Task.FromResult(IdentityResult.Failed(new IdentityError { Description = ex.Message }));
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: 释放托管状态(托管对象)
                }

                // TODO: 释放未托管的资源(未托管的对象)并重写终结器
                // TODO: 将大型字段设置为 null
                disposedValue = true;
            }
        }

        // // TODO: 仅当“Dispose(bool disposing)”拥有用于释放未托管资源的代码时才替代终结器
        // ~FlowRoleStore()
        // {
        //     // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
