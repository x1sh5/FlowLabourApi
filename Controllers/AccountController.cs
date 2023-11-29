using FlowLabourApi.Authentication;
using FlowLabourApi.Config;
using FlowLabourApi.Models;
using FlowLabourApi.Models.context;
using FlowLabourApi.Models.state;
using FlowLabourApi.Options;
using FlowLabourApi.Utils;
using FlowLabourApi.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Text.RegularExpressions;

namespace FlowLabourApi.Controllers
{
    /// <summary>
    /// 账户管理
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly FlowContext _context;
        private readonly JwtOptions _jwtOptions;
        //private readonly IUserStore<AuthUser> _flowUserStore;
        //private SignInManager<AuthUser> signInManager;
        private readonly IAuthTokenService _authTokenService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="jwtOptions"></param>
        /// <param name="authTokenService"></param>
        public AccountController(FlowContext context,
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
        /// 是否登录验证
        /// </summary>
        /// <returns></returns>
        [HttpHead("loginTest")]
        [Authorize]
        public void LoginTest()
        {
            Response.StatusCode = 200;
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

            var nameOk = NameCheck(register.UserName);
            var namecheck = (ResponeMessage<CheckMSg>)nameOk.Value;
            if (!namecheck.Data.Status)
            {
                return BadRequest(new { status = false, message = namecheck.Data.msg });
            }

            var pwOk = PassWordCheck(register.Password);
            if (!pwOk.Status)
            {
                return BadRequest(new { status = false, message = pwOk.msg });
            }

            if (!string.IsNullOrEmpty(register.Email))
            {
                var emailOk = EmailCheck(register.Email);
                var isOk = (ResponeMessage<CheckMSg>)emailOk.Value;
                if (!isOk.Data.Status)
                {
                    return BadRequest(new { status = false, message = isOk.Data.msg });
                }
            }
            if (!string.IsNullOrEmpty(register.PhoneNo))
            {
                var phoneOk = PhoneCheck(register.PhoneNo);
                var phonecheck = (ResponeMessage<CheckMSg>)phoneOk.Value;
                if (!phonecheck.Data.Status)
                {
                    return BadRequest(new { status = false, message = phonecheck.Data.msg });
                }
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

            return Ok(new ResponeMessage<string> { ORCode=200,Message="注册成功"});
        }

        /// <summary>
        /// 用户名检查
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        [HttpGet("namecheck")]
        [AllowAnonymous]
        public ObjectResult NameCheck(string username)
        {
            if (string.IsNullOrEmpty(username))
            {
                return BadRequest(new ResponeMessage<CheckMSg> { ORCode = 400, Message = "用户名无效"
                ,Data=new CheckMSg(false, "用户名无效")
                });
            }
            var user = _context.AuthUsers.FirstOrDefault(e => e.UserName == username);
            var rsm = new ResponeMessage<CheckMSg> { ORCode = 200, Message = "" };
            if (user != null)
            {
                rsm.Data = new CheckMSg(false, "用户名已存在");
                return Ok(rsm);
            }
            //Response.Headers
            // code to handle registration
            rsm.Data = new CheckMSg(true, "用户名通过");
            return Ok(rsm);
        }

        [NonAction]
        private CheckMSg PassWordCheck(string pw)
        {
            if (string.IsNullOrEmpty(pw))
            {
                return new CheckMSg(false, "密码无效");
            }
            if (pw.Length < 8)
            {
                return new CheckMSg(false, "密码长度不能小于8位");
            }
            if (pw.Length > 16)
            {
                return new CheckMSg(false, "密码长度不能大于16位");
            }
            
            var regex = new Regex(@"^(?=.*[0-9])(?=.*[A-Z])(?=.*\W).{8,16}$");
            if (!regex.IsMatch(pw))
            {
                return new CheckMSg(false, "密码格式不符合要求,必须包含特殊字符，大写字母和数字");
            }

            return new CheckMSg(true, "密码通过");
        }

        /// <summary>
        /// 电话检查
        /// </summary>
        /// <param name="phoneNo"></param>
        /// <returns></returns>
        [HttpGet("phonecheck")]
        [AllowAnonymous]
        public ObjectResult PhoneCheck(string phoneNo)
        {
            var rsm = new ResponeMessage<CheckMSg> { ORCode = 200, Message = "" };
            if (!IdentityValidateUtil.ValidatePhoneFormat(phoneNo))
            {
                rsm.Data = new CheckMSg(false, "电话号码无效！");
                return BadRequest(rsm);
            }

            var user = _context.AuthUsers.FirstOrDefault(e => e.PhoneNo == phoneNo);
            if (user != null)
            {
                rsm.Data = new CheckMSg(false, "该电话号码以绑定其他账号");
                return Ok(rsm);
            }

            rsm.Data = new CheckMSg(true, "可用");
            return Ok(rsm);
        }

        /// <summary>
        /// 邮件检查
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        [HttpGet("emailcheck")]
        [AllowAnonymous]
        public ObjectResult EmailCheck(string email)
        {
            var isok = IdentityValidateUtil.ValidateEmailFormat(email);
            var rsm = new ResponeMessage<CheckMSg> { ORCode = 200,Message="" };
            if (!isok)
            {
                rsm.Data = new CheckMSg(false, "邮件格式无效");
                return Ok(rsm);
            }
            var user = _context.AuthUsers.FirstOrDefault(e => e.Email == email);
            if (user != null)
            {
                rsm.Data = new CheckMSg(false, "该邮件以绑定其他账号");
                return Ok(rsm);
            }

            rsm.Data = new CheckMSg(true, "可用");
            return Ok(rsm);
        }

        /// <summary>
        /// 默认重定向地址
        /// </summary>
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
                return Unauthorized("用户身份未通过验证。");
            }

            var UA = Request.Headers.UserAgent[0];

            var ua = HashUtil.Md5(UA ?? "unkownAgent");

            UserRole? userRole = _context.UserRoles.Include(o => o.Role).FirstOrDefault(e => e.UserId == user.Id);
            if (userRole == null)
            {
                return Unauthorized("用户身份未通过验证。");
            }
            userRole.User = user;

            //SecurityToken? token = GenerateToken(userRole);
            AuthTokenDto? token = await _authTokenService.CreateAuthTokenAsync(userRole, ua,_context);

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

            UserView userView = new UserView
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                PhoneNo = user.PhoneNo,
                Avatar = user.Avatar,
                AccessToken = accessTokenstr,
                RefreshToken = refreshTokenstr
            };

            return Ok(userView);
        }

        /// <summary>
        /// 查询是否有default权限
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("special/{id}")]
        [Authorize()]
        [ApiExplorerSettings(IgnoreApi = true)]
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
        [AllowAnonymous]
        public async Task<IActionResult> Logout()
        {
            var id = User.FindFirstValue(JwtClaimTypes.IdClaim);
            var UA = Request.Headers.UserAgent[0];
            var ua = HashUtil.Md5(UA ?? "unkownAgent");
            var ip = HttpContext.Connection.RemoteIpAddress.ToString();

            var userToken = await _context.UserTokens
                .FirstOrDefaultAsync(t => t.LoginProvider == ua && t.UserId == int.Parse(id));
            if (userToken != null)
            {
                userToken.Expires = DateTime.Now;
                await _context.SaveChangesAsync();
            }
            return NoContent();
        }

        /// <summary>
        /// 刷新token
        /// </summary>
        /// <param name="accessToken"></param>
        /// <param name="refreshToken"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost("refresh-token")]
        [HttpGet("refresh-token")]
        public async Task<IActionResult> RefreshToken([Required]string accessToken, 
            [Required]string refreshToken)
        {
            AuthTokenDto dto = new AuthTokenDto { AccessToken = accessToken, RefreshToken = refreshToken };
            try
            {
                var token = await _authTokenService.RefreshAuthTokenAsync(dto,_context);
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
                return Ok(new { AccessToken = accessTokenstr, RefreshToken = refreshTokenstr});
            }
            catch (BadHttpRequestException ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        /// <summary>
        /// 注销账号
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        [HttpPost("unregister")]
        [Authorize]
        public IActionResult UnRegister([FromForm]string password)
        {
            var id = User.Claims.FirstOrDefault(User => User.Type == JwtClaimTypes.IdClaim).Value;
            var user = _context.AuthUsers.Find(int.Parse(id));
            if(user.Passwordhash == HashUtil.Sha256(password))
            {
                return Ok("已成功发起注销申请，成功后会通过邮件通知。");
            }
            return BadRequest("密码错误");
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

    internal record CheckMSg(bool Status, string msg);

    internal record UserCheck(string password, string msg, string token);
}
