using FlowLabourApi.Config;

namespace FlowLabourApi.Authentication
{
    public class BlacklistMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IBlacklistService _blacklistService;

        public BlacklistMiddleware(RequestDelegate next, IBlacklistService blacklistService)
        {
            _next = next;
            _blacklistService = blacklistService;
        }

        public async Task Invoke(HttpContext context)
        {
            
            var user = context.User;
            if (user != null)
            {
                var id = user.Claims.FirstOrDefault(x => x.Type == JwtClaimTypes.IdClaim)?.Value;
                if(!string.IsNullOrEmpty(id))
                {
                    var isBlacklisted = _blacklistService.IsBlacklisted(id);
                    if (isBlacklisted)
                    {
                        context.Response.StatusCode = 401;
                        await context.Response.WriteAsync("Unauthorized");
                        return;
                    }
                }

            }

            await _next.Invoke(context);
        }
    }
}
