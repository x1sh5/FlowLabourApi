using FlowLabourApi.Models;
using FlowLabourApi.ViewModels;

namespace FlowLabourApi.Authentication
{
    public interface IAuthTokenService
    {
        Task<AuthTokenDto> CreateAuthTokenAsync(UserRole ur, string loginProvider);

        Task<AuthTokenDto> RefreshAuthTokenAsync(AuthTokenDto token);
    }
}
