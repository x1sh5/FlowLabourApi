using FlowLabourApi.Models;
using FlowLabourApi.Models.context;
using FlowLabourApi.ViewModels;

namespace FlowLabourApi.Authentication
{
    public interface IAuthTokenService
    {
        Task<AuthTokenDto> CreateAuthTokenAsync(UserRole ur, string loginProvider, XiangxpContext _dbContext);

        Task<AuthTokenDto> RefreshAuthTokenAsync(AuthTokenDto token, XiangxpContext _dbContext);
    }
}
