using FlowLabourApi.Config;
using FlowLabourApi.Models;
using FlowLabourApi.Models.context;
using FlowLabourApi.Options;
using FlowLabourApi.ViewModels;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Win32;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;

namespace FlowLabourApi.Authentication
{
    public class AuthTokenService : IAuthTokenService
    {
        private const string RefreshTokenIdClaimType = "refresh_token_id";

        private readonly JwtBearerOptions _jwtBearerOptions;
        private readonly JwtOptions _jwtOptions;
        //private readonly SigningCredentials _signingCredentials;
        private readonly ILogger<AuthTokenService> _logger;
        //private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthTokenService(
           IOptionsSnapshot<JwtBearerOptions> jwtBearerOptions,
           //IOptionsSnapshot<JwtOptions> jwtOptions,
           FlowContext dbContext,
           ILogger<AuthTokenService> logger)
        {
            _jwtBearerOptions = jwtBearerOptions.Get(JwtBearerDefaults.AuthenticationScheme);
            _jwtOptions = new JwtOptions();
            _logger = logger;
            //_httpContextAccessor = httpContextAccessor;
        }

        public async Task<AuthTokenDto> CreateAuthTokenAsync(UserRole ur, string loginProvider, FlowContext _dbContext)
        {
            var result = new AuthTokenDto();

            string? refreshToken = await CreateRefreshTokenAsync(ur.UserId, loginProvider,_dbContext);
            result.RefreshToken = refreshToken;
            result.AccessToken = CreateAccessToken(ur, loginProvider);

            // 将Jwt放入Cookie，这样H5就无需在Header中Jwt传递了（主要是省事）
            //_httpContextAccessor.HttpContext.Response.Cookies.Append(HeaderNames.Authorization, result.AccessToken, new CookieOptions
            //{
            //    // 本地环境，忽略域
            //    //Domain = "",
            //    HttpOnly = true,
            //    IsEssential = true,
            //    MaxAge = TimeSpan.FromDays(_jwtOptions.RefreshTokenExpiresDays),
            //    Path = "/",
            //    SameSite = SameSiteMode.Lax
            //});

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="useragent_bs64">UA 通过base64后得到的字符串 </param>
        /// <returns></returns>
        private async Task<string> CreateRefreshTokenAsync(int userId, string useragent_bs64, FlowContext _dbContext)
        {
            //var tokenId = Guid.NewGuid().ToString("N");

            var rnBytes = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(rnBytes);
            var token = Convert.ToBase64String(rnBytes);

            var userToken = await _dbContext.UserTokens.FirstOrDefaultAsync(t => t.LoginProvider == useragent_bs64 && t.UserId == userId);
            if (userToken != null)
            {
                userToken.RefreshToken = token;
                userToken.Modify = DateTime.Now;
                userToken.Expires = DateTime.Now + _jwtOptions.RefreshTokenExpires;
                _dbContext.SaveChanges();
                return token;
            }

            await _dbContext.UserTokens.AddAsync(new UserToken
            {
                LoginProvider = useragent_bs64,
                UserId = userId,
                RefreshToken = token,
                Modify = DateTime.Now,
                Expires = DateTime.Now + _jwtOptions.RefreshTokenExpires,
            });
            _dbContext.SaveChanges();

            return token;
        }

        private string? CreateAccessToken(UserRole userRole, string loginProvider)
        {
            List<Claim>? claims = new List<Claim>();
            claims.Add(new Claim(JwtClaimTypes.IdClaim, userRole.User.Id.ToString()));
            claims.Add(new Claim(JwtClaimTypes.NameClaim, userRole.User.UserName));
            claims.Add(new Claim(JwtClaimTypes.RoleClaim, userRole.Role.Privilege));
            claims.Add(new Claim(JwtClaimTypes.LoginProviderClaim, loginProvider));
            SecurityKey? secret = _jwtOptions.SecurityKey;
            var identity = new ClaimsIdentity(claims, JwtBearerDefaults.AuthenticationScheme,
                JwtBearerDefaults.AuthenticationScheme, userRole.Role.Privilege);
            //await HttpContext.SignInAsync(JwtBearerDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));


            JwtSecurityTokenHandler? tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken? token = tokenHandler.CreateToken(new SecurityTokenDescriptor()
            {
                Subject = identity,
                Issuer = _jwtOptions.Issuer,
                Audience = _jwtOptions.Audience,
                Expires = DateTime.Now + _jwtOptions.AccessTokenExpires,
                SigningCredentials = new SigningCredentials(secret, SecurityAlgorithms.HmacSha256)
            });
            var handler = new JwtSecurityTokenHandler();
            var accessToken = handler.WriteToken(token);
            //tokenHandler.WriteToken(token);

            return accessToken;
        }

        public async Task<AuthTokenDto> RefreshAuthTokenAsync(AuthTokenDto token, FlowContext _dbContext)
        {
            var validationParameters = _jwtBearerOptions.TokenValidationParameters.Clone();
            validationParameters.ValidateLifetime = false;

            var handler = _jwtBearerOptions.SecurityTokenValidators.OfType<JwtSecurityTokenHandler>().FirstOrDefault()
                ?? new JwtSecurityTokenHandler();
            ClaimsPrincipal principal = null;
            try
            {
                principal = handler.ValidateToken(token.AccessToken, validationParameters, out _);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex.ToString());
                throw new BadHttpRequestException("Invalid access token");
            }

            //decode jwt
            ClaimsIdentity? identity = principal.Identities.First();
            //var name = identity.Claims.FirstOrDefault(c => c.Type == JwtClaimTypes.NameClaim).Value;
            string? userId = identity.Claims.FirstOrDefault(c => c.Type == JwtClaimTypes.IdClaim).Value;
            string? loginProvider = identity.Claims
                .FirstOrDefault(c => c.Type == JwtClaimTypes.LoginProviderClaim)?.Value;

            //var refreshTokenKey = GetRefreshTokenKey(name, loginProvider);
            UserToken? refreshToken = await _dbContext.UserTokens
                .Where(t => t.UserId == int.Parse(userId) && t.LoginProvider == loginProvider)
                .FirstOrDefaultAsync();
            if (refreshToken==null || Uri.EscapeDataString(refreshToken.RefreshToken) != token.RefreshToken
                || refreshToken.IsExpired)
            {
                throw new BadHttpRequestException("Invalid refresh token");
            }

            var userRole = await _dbContext.UserRoles
                .Include(ur => ur.User)
                .Include(ur => ur.Role)
                .FirstOrDefaultAsync(ur => ur.UserId == int.Parse(userId));

            return await CreateAuthTokenAsync(userRole, loginProvider,_dbContext);
        }

        private string GetRefreshTokenKey(string name, string refreshTokenId)
        {
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException(nameof(name));
            if (string.IsNullOrEmpty(refreshTokenId)) throw new ArgumentNullException(nameof(refreshTokenId));

            return $"{name}:{refreshTokenId}";
        }

    }
}
