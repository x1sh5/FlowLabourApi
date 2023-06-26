using FlowLabourApi.Authentication;
using FlowLabourApi.Models;
using FlowLabourApi.Models.context;
using FlowLabourApi.Options;
using FlowLabourApi.Utils;
using FlowLabourApi.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using NuGet.Common;
using NuGet.Protocol.Plugins;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;

namespace FlowLabourApi.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly XiangxpContext _context;
        private JwtOptions _jwtOptions;
        //private readonly IUserStore<AuthUser> _flowUserStore;
        private SignInManager<AuthUser> signInManager;

        public AccountController(XiangxpContext context, SignInManager<AuthUser> signInManager,
            IOptionsSnapshot<JwtOptions> jwtOptions
             /* FlowUserStore flowUserStore */)
        {
            _context = context;
            this.signInManager = signInManager;
            _jwtOptions = jwtOptions.Value;
            //_flowUserStore = flowUserStore;
        }
        /// <summary>
        /// ×¢²á
        /// </summary>
        /// <param name="register"></param>
        /// <returns></returns>
        [HttpPost("register")]
        [AllowAnonymous]
        public IActionResult Register([FromBody] RegisterView register)
        {
            // code to handle registration
            return Ok();
        }

        /// <summary>
        /// µÇÂ¼ÌáÊ¾
        /// </summary>
        /// <param name="login"></param>
        /// <returns></returns>
        [HttpGet("login")]
        //[HttpPost("Login")]
        [AllowAnonymous]
        public IActionResult Login()
        {
            return Unauthorized("ÇëµÇÂ¼ºóÔÙÊÔ£¡");
        }

        ///// <summary>
        ///// µÇÂ¼ÌáÊ¾
        ///// </summary>
        ///// <param name="login"></param>
        ///// <returns></returns>
        //[HttpPost("login")]
        ////[HttpPost("Login")]
        //[AllowAnonymous]
        //public IActionResult Login([FromBody] LoginView login)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }
        //    if (login == null)
        //    {
        //        return BadRequest("Invalid client request");
        //    }
        //    var user = _context.AuthUsers.FirstOrDefault(
        //        e => e.UserName == login.UserName &&
        //        e.Passwordhash == HashUtil.GetHash(login.Password)
        //    );
        //    if (user == null)
        //    {
        //        return Unauthorized();
        //    }
        //    var tokenString = Token.GenerateJSONWebToken(login.UserName);
        //    return Ok(new { token = tokenString });
        //}

        /// <summary>
        /// µÇÂ¼
        /// </summary>
        /// <param name="login"></param>
        /// <returns></returns>
        [HttpPost("login")]
        //[HttpPost("Login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginView login)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if(login == null)
            {
                return BadRequest("Invalid client request");
            }
            //var user = await _flowUserStore.FindByLoginViewAsync(login,CancellationToken.None);
            var user = _context.AuthUsers.FirstOrDefault(
                e => e.UserName == login.UserName &&
                e.Passwordhash == HashUtil.GetHash(login.Password)
               );
            if (user == null)
            {
                return Unauthorized();
            }
            UserRole? role = _context.Userroles.Include(o=>o.Role).FirstOrDefault(e => e.UserId == user.Id);
                //.FirstOrDefault(e => e.UserId== user.Id);

            //await HttpContext.SignInAsync(JwtBearerDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));
            await signInManager.SignInAsync(user, true);

            SecurityToken? token = GenerateToken(login.UserName, role);

            // code to handle login
            return Ok(new { V = new JwtSecurityTokenHandler().WriteToken(token) });
        }

        [HttpGet("special")]
        [Authorize()]
        public IActionResult Edit(int id)
        {
            ClaimsIdentity? claims = User.Identities.FirstOrDefault(x=>x.Claims.Contains(new Claim("Role", "default")));
            return Ok(claims);
        }

        /// <summary>
        /// ×¢Ïú
        /// </summary>
        /// <returns></returns>
        [HttpPost("logout")]
        [Authorize]
        public IActionResult Logout()
        {
            // code to handle logout
            bool isAuth = User.Identity.IsAuthenticated;
            return Content($"user is {isAuth}");
        }

        [NonAction]
        private SecurityToken? GenerateToken(string userName,UserRole role)
        {
            RsaSecurityKey? secret = new RsaSecurityKey(RSA.Create());
            List<Claim>? clams = new List<Claim>();
            clams.Add(new Claim("UserName", userName));
            clams.Add(new Claim("Role", role.Role.Privilege));
            var identity = new ClaimsIdentity(clams, JwtBearerDefaults.AuthenticationScheme);
            //await HttpContext.SignInAsync(JwtBearerDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));


            JwtSecurityTokenHandler? tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken? token = tokenHandler.CreateToken(new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim("Name", userName),
                    new Claim("Role", role.Role.Privilege)
                }),
                Issuer = _jwtOptions.Issuer,
                Audience = _jwtOptions.Audience,
                Expires = DateTime.UtcNow.AddMinutes(_jwtOptions.ExpiresMinutes),
                SigningCredentials = new SigningCredentials(secret, SecurityAlgorithms.RsaSha256)
            });
            //tokenHandler.WriteToken(token);

            return token;
        }


    }
}
