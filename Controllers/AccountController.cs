using FlowLabourApi.Models;
using FlowLabourApi.Models.context;
using FlowLabourApi.Utils;
using FlowLabourApi.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using NuGet.Common;
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
        private SignInManager<AuthUser> signInManager;

        public AccountController(XiangxpContext context, SignInManager<AuthUser> signInManager)
        {
            _context = context;
            this.signInManager = signInManager;
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
            RsaSecurityKey? secret = new RsaSecurityKey(RSA.Create());
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if(login == null)
            {
                return BadRequest("Invalid client request");
            }
            var user = _context.AuthUsers.FirstOrDefault(
                e =>  e.UserName == login.UserName &&
                e.Passwordhash == HashUtil.GetHash(login.Password)
               );
            if(user == null)
            {
                return Unauthorized();
            }
            Userrole? role = _context.Userroles.FirstOrDefault(e => e.Userid== user.Id);
            //
            List<Claim>? clams = new List<Claim>();
            clams.Add(new Claim("UserName", login.UserName));
            clams.Add(new Claim("Role", role.Role.Privilege));
            var identity = new ClaimsIdentity(clams, JwtBearerDefaults.AuthenticationScheme);
            //await HttpContext.SignInAsync(JwtBearerDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));
            await signInManager.SignInAsync(user, true);


            JwtSecurityTokenHandler? tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken? token = tokenHandler.CreateToken(new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, login.UserName),
                    new Claim(ClaimTypes.Role, role.Role.Privilege)
                }),
                SigningCredentials = new SigningCredentials(secret, SecurityAlgorithms.RsaSha256)
            });
            

            // code to handle login
            return Ok(token);
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


    }
}
