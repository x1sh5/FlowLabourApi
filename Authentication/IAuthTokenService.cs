using FlowLabourApi.Models;
using FlowLabourApi.ViewModels;

namespace FlowLabourApi.Authentication
{
    public interface IAuthTokenService
    {
        Task<AuthTokenDto> CreateAuthTokenAsync(UserRole ur);

        Task<AuthTokenDto> RefreshAuthTokenAsync(AuthTokenDto token);
    }
}
