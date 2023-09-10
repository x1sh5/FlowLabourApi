using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace FlowLabourApi.Authentication
{
    public class LoginProviderTokenHandler : ISecurityTokenValidator
    {
        private JwtSecurityTokenHandler _tokenHandler;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private int _maxTokenSizeInBytes = TokenValidationParameters.DefaultMaximumTokenSizeInBytes;

        public bool CanValidateToken
        {
            get { return true; }
        }

        public int MaximumTokenSizeInBytes { 
            get{
                return _maxTokenSizeInBytes;
            }
            set{ 
                _maxTokenSizeInBytes = value;
            }
        }

        public LoginProviderTokenHandler(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            _tokenHandler = new JwtSecurityTokenHandler();
        }

        public bool CanReadToken(string securityToken)
        {
            return _tokenHandler.CanReadToken(securityToken);
        }

        public ClaimsPrincipal ValidateToken(string securityToken, TokenValidationParameters validationParameters, out SecurityToken validatedToken)
        {
            var httpContext = _httpContextAccessor.HttpContext;
            
            var principal = _tokenHandler.ValidateToken(securityToken, validationParameters, out validatedToken);

            return principal;
        }
    }
}
