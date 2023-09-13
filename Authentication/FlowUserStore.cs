using FlowLabourApi.Models;
using FlowLabourApi.Models.context;
using FlowLabourApi.Utils;
using FlowLabourApi.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FlowLabourApi.Authentication
{
    public class FlowUserStore : IUserStore<AuthUser>, IDisposable
    {
        private bool disposedValue;
        private readonly FlowContext _context;
        private readonly ILogger<FlowUserStore> _logger;

        public FlowUserStore(FlowContext context, ILogger<FlowUserStore> logger, bool disposedValue = false)
        {
            this.disposedValue = disposedValue;
            _context = context;
            _logger = logger;
        }

        public async Task<IdentityResult> CreateAsync(AuthUser user, CancellationToken cancellationToken)
        {
            Console.Out.WriteLine("FlowUserStore.CreateAsync run");
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            try
            {
                _ = await _context.AuthUsers.AddAsync(user, cancellationToken);
                _ = await _context.SaveChangesAsync(cancellationToken);

                return await Task.FromResult(IdentityResult.Success);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create user");

                return await Task.FromResult(IdentityResult.Failed(new IdentityError
                {
                    Code = "FailedToCreateUser",
                    Description = "Failed to create user"
                }));
            }
        }


        public async Task<IdentityResult> DeleteAsync(AuthUser user, CancellationToken cancellationToken)
        {
            Console.Out.WriteLine("FlowUserStore.DeleteAsync run");
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            try
            {
                _context.AuthUsers.Remove(user);
                await _context.SaveChangesAsync(cancellationToken);

                return await Task.FromResult(IdentityResult.Success);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete user");

                return await Task.FromResult(IdentityResult.Failed(new IdentityError
                {
                    Code = "FailedToDeleteUser",
                    Description = "Failed to delete user"
                }));
            }
        }


        public async Task<AuthUser> FindByIdAsync(int userId, CancellationToken cancellationToken)
        {
            Console.Out.WriteLine("FlowUserStore.FindByIdAsync run");
            var user = await _context.AuthUsers.FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
            if (user == null) throw new ArgumentNullException(nameof(user));
            return user;
        }

        public async Task<AuthUser> FindByLoginViewAsync(LoginView login, CancellationToken cancellationToken)
        {
            Console.Out.WriteLine("FlowUserStore.FindByLoginViewAsync run");
            var user = await _context.AuthUsers.FirstOrDefaultAsync(
                e => e.UserName == login.UserName &&
                e.Passwordhash == HashUtil.Sha256(login.Password),
                cancellationToken
               );
            if (user == null) throw new ArgumentNullException(nameof(user));
            return user;
        }

        public async Task<AuthUser> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
        {
            Console.Out.WriteLine("FlowUserStore.FindByNameAsync run");
            var user = await _context.AuthUsers.FirstOrDefaultAsync(u => u.UserName == normalizedUserName, cancellationToken);
            if (user == null) throw new ArgumentNullException(nameof(user));
            return user;
        }

        public Task<string> GetNormalizedUserNameAsync(AuthUser user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public async Task<string> GetUserIdAsync(AuthUser user, CancellationToken cancellationToken)
        {
            Console.Out.WriteLine("FlowUserStore.GetUserIdAsync run");
            var user1 = await FindByIdAsync(user.Id, cancellationToken);
            return user1.Id.ToString();
        }

        public async Task<string> GetUserNameAsync(AuthUser user, CancellationToken cancellationToken)
        {
            Console.Out.WriteLine("FlowUserStore.GetUserNameAsync run");
            var user1 = await FindByIdAsync(user.Id, cancellationToken);
            return user1.UserName;
        }

        public Task SetNormalizedUserNameAsync(AuthUser user, string normalizedName, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }


        public async Task<IdentityResult> UpdateAsync(AuthUser user, CancellationToken cancellationToken)
        {
            Console.Out.WriteLine("FlowUserStore.UpdateAsync run");
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            var existingUser = await _context.AuthUsers.FirstOrDefaultAsync(u => u.Id == user.Id, cancellationToken);
            if (existingUser == null)
            {
                return IdentityResult.Failed(new IdentityError
                {
                    Code = "UserNotFound",
                    Description = $"Could not find user with Id '{user.Id}'"
                });
            }

            existingUser.UserName = user.UserName;
            existingUser.Email = user.Email;
            existingUser.PhoneNo = user.PhoneNo;

            try
            {
                _ = _context.AuthUsers.Update(existingUser);
                _ = await _context.SaveChangesAsync(cancellationToken);

                return await Task.FromResult(IdentityResult.Success);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update user");

                return await Task.FromResult(IdentityResult.Failed(new IdentityError
                {
                    Code = "FailedToUpdateUser",
                    Description = "Failed to update user"
                }));
            }
        }

        public async Task SetUserNameAsync(AuthUser user, string userName, CancellationToken cancellationToken)
        {
            Console.Out.WriteLine("FlowUserStore.SetUserNameAsync run");
            user.UserName = userName;
            await UpdateAsync(user, cancellationToken);
        }

        public async Task<AuthUser> FindByIdAsync(string userId, CancellationToken cancellationToken)
        {
            Console.Out.WriteLine("FlowUserStore.FindByIdAsync run");
            int? id;
            id = int.TryParse(userId, out int result) ? result : null;
            if (id == null) throw new ArgumentNullException(nameof(userId));
            var user = await _context.AuthUsers.FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            return user;
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
        // ~FlowUserStore()
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
