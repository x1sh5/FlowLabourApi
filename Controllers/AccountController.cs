using FlowLabourApi.Authentication;
using FlowLabourApi.Config;
using FlowLabourApi.Models;
using FlowLabourApi.Models.context;
using FlowLabourApi.Options;
using FlowLabourApi.Utils;
using FlowLabourApi.ViewModels;
using Google.Protobuf.WellKnownTypes;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using System.Security.Claims;

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
        //private SignInManager<AuthUser> signInManager;
        private readonly IAuthTokenService _authTokenService;

        public AccountController(XiangxpContext context,
            //SignInManager<AuthUser> signInManager,
            IOptionsSnapshot<JwtOptions> jwtOptions,
            IAuthTokenService authTokenService
             /* FlowUserStore flowUserStore */)
        {
            _context = context;
            //this.signInManager = signInManager;
            _jwtOptions = jwtOptions.Value;
            _authTokenService = authTokenService;
            //_flowUserStore = flowUserStore;
        }
        /// <summary>
        /// 注册
        /// </summary>
        /// <param name="register"></param>
        /// <returns></returns>
        [HttpPost("register")]
        [AllowAnonymous]
        public IActionResult Register([FromBody] RegisterView register)
        {
            bool validate = false;
            EmailCheck(register.Email, out validate);
            if (!validate)
            {
                return BadRequest(new { status = false, message = "邮件无效！" });
            }
            NameCheck(register.UserName, out validate);
            if (!validate)
            {
                return BadRequest(new { status = false, message = "用户名已存在！" });
            }
            PhoneCheck(register.PhoneNo, out validate);
            if (!validate)
            {
                return BadRequest(new { status = false, message = "电话号码已被注册！" });
            }

            var user = new AuthUser
            {
                UserName = register.UserName,
                Passwordhash = HashUtil.Sha256(register.Password),
                Email = register.Email,
                PhoneNo = register.PhoneNo
            };
            var e = _context.AuthUsers.Add(user);
            _context.SaveChanges();
            _context.UserRoles.Add(new UserRole { RoleId = 1, UserId = e.Entity.Id });
            _context.SaveChanges();

            return Ok();
        }

        /// <summary>
        /// 用户名检查
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        [HttpGet("namecheck")]
        [AllowAnonymous]
        public IActionResult NameCheck(string username, out bool validate)
        {
            var user = _context.AuthUsers.FirstOrDefault(e => e.UserName == username);
            if (user != null)
            {
                validate = false;
                return Ok(new { status = false, message = "用户名已存在" });
            }
            //Response.Headers
            // code to handle registration
            validate = true;
            return Ok(new { status = true, message = "用户名通过" });
        }

        /// <summary>
        /// 电话检查
        /// </summary>
        /// <param name="PhoneNo"></param>
        /// <returns></returns>
        [HttpGet("phonecheck")]
        [AllowAnonymous]
        public IActionResult PhoneCheck(string PhoneNo, out bool validate)
        {
            var user = _context.AuthUsers.FirstOrDefault(e => e.PhoneNo == PhoneNo);
            if (user != null)
            {
                validate = false;
                return Ok(new { status = false, message = "该电话号码以绑定其他账号" });
            }

            validate = true;
            return Ok(new { status = true, message = "可用" });
        }

        /// <summary>
        /// 邮件检查
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        [HttpGet("emailcheck")]
        [AllowAnonymous]
        public IActionResult EmailCheck(string email, out bool validate)
        {
            var isok = IdentityValidateUtil.ValidateEmailFormat(email);
            if (!isok)
            {
                validate = false;
                return Ok(new { status = false, message = "邮件格式无效" });
            }
            var user = _context.AuthUsers.FirstOrDefault(e => e.Email == email);
            if (user != null)
            {
                validate = false;
                return Ok(new { status = false, message = "该邮件以绑定其他账号" });
            }

            validate = true;
            return Ok(new { status = true, message = "可用" });
        }

        /// <summary>
        /// 默认重定向地址
        /// </summary>
        /// <param name="login"></param>
        /// <returns></returns>
        // 直接以/开头的路由，会当成决对路径。
        [HttpGet("/Account/Login")]
        [AllowAnonymous]
        public IActionResult Login()
        {
            return Unauthorized("请登录后再试！");
        }


        /// <summary>
        /// 登录
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
            if (login == null)
            {
                return BadRequest("Invalid client request");
            }
            //var user = await _flowUserStore.FindByLoginViewAsync(login,CancellationToken.None);
            var user = _context.AuthUsers.FirstOrDefault(
                e => e.UserName == login.UserName &&
                e.Passwordhash == HashUtil.Sha256(login.Password)
               );
            if (user == null)
            {
                return Unauthorized();
            }
            UserRole? userRole = _context.UserRoles.Include(o => o.Role).FirstOrDefault(e => e.UserId == user.Id);
            if (userRole == null)
            {
                return Unauthorized("用户身份未通过验证。");
            }
            userRole.User = user;

            //await HttpContext.SignInAsync(JwtBearerDefaults.AuthenticationScheme, User);

            //await signInManager.SignInAsync(user, true);

            var UA = Request.Headers.UserAgent[0];

            var ua = HashUtil.Md5(UA ?? "unkownAgent");

            //SecurityToken? token = GenerateToken(userRole);
            AuthTokenDto? token = await _authTokenService.CreateAuthTokenAsync(userRole, ua);

            CookieOptions cookieOptions = new CookieOptions
            {
                //HttpOnly = true,
                Expires = DateTime.Now.AddHours(18),
                Path = "/"
            };
            Response.Cookies.Append(CookieTypes.accessToken, token.AccessToken, cookieOptions);
            Response.Cookies.Append(CookieTypes.refreshToken, token.RefreshToken, cookieOptions);

            string accessTokenstr = cookiestr(CookieTypes.accessToken, token.AccessToken, cookieOptions);
            string refreshTokenstr = cookiestr(CookieTypes.refreshToken, token.RefreshToken, cookieOptions);
            token.AccessToken = accessTokenstr;
            token.RefreshToken = refreshTokenstr;
            return Ok(token);
        }

        [HttpGet("special")]
        [Authorize()]
        public IActionResult Edit(int id)
        {
            var u = User;
            ClaimsIdentity? claims = User.Identities.FirstOrDefault(x => x.Claims.Contains(new Claim("Role", "default")));
            return Ok(claims);
        }

        /// <summary>
        /// 注销
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

        [AllowAnonymous]
        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] AuthTokenDto dto)
        {
            try
            {
                var token = await _authTokenService.RefreshAuthTokenAsync(dto);

                return Ok(token);
            }
            catch (BadHttpRequestException ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        [NonAction]
        public string cookiestr(string key,string value, CookieOptions options)
        {
            bool _enableCookieNameEncoding = AppContext.TryGetSwitch("Microsoft.AspNetCore.Http.EnableCookieNameEncoding", out var isEnabled) && isEnabled;
            string value2 = new SetCookieHeaderValue(_enableCookieNameEncoding ? Uri.EscapeDataString(key) : key, Uri.EscapeDataString(value))
            {
                Domain = options.Domain,
                Path = options.Path,
                Expires = options.Expires,
                MaxAge = options.MaxAge,
                Secure = options.Secure,
                SameSite = (Microsoft.Net.Http.Headers.SameSiteMode)options.SameSite,
                HttpOnly = options.HttpOnly
            }.ToString();

            return value2;
        }

    }
}
