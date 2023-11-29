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
    /// �˻�����
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
        /// �Ƿ��¼��֤
        /// </summary>
        /// <returns></returns>
        [HttpHead("loginTest")]
        [Authorize]
        public void LoginTest()
        {
            Response.StatusCode = 200;
        }

        /// <summary>
        /// ע��
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

            return Ok(new ResponeMessage<string> { ORCode=200,Message="ע��ɹ�"});
        }

        /// <summary>
        /// �û������
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        [HttpGet("namecheck")]
        [AllowAnonymous]
        public ObjectResult NameCheck(string username)
        {
            if (string.IsNullOrEmpty(username))
            {
                return BadRequest(new ResponeMessage<CheckMSg> { ORCode = 400, Message = "�û�����Ч"
                ,Data=new CheckMSg(false, "�û�����Ч")
                });
            }
            var user = _context.AuthUsers.FirstOrDefault(e => e.UserName == username);
            var rsm = new ResponeMessage<CheckMSg> { ORCode = 200, Message = "" };
            if (user != null)
            {
                rsm.Data = new CheckMSg(false, "�û����Ѵ���");
                return Ok(rsm);
            }
            //Response.Headers
            // code to handle registration
            rsm.Data = new CheckMSg(true, "�û���ͨ��");
            return Ok(rsm);
        }

        [NonAction]
        private CheckMSg PassWordCheck(string pw)
        {
            if (string.IsNullOrEmpty(pw))
            {
                return new CheckMSg(false, "������Ч");
            }
            if (pw.Length < 8)
            {
                return new CheckMSg(false, "���볤�Ȳ���С��8λ");
            }
            if (pw.Length > 16)
            {
                return new CheckMSg(false, "���볤�Ȳ��ܴ���16λ");
            }
            
            var regex = new Regex(@"^(?=.*[0-9])(?=.*[A-Z])(?=.*\W).{8,16}$");
            if (!regex.IsMatch(pw))
            {
                return new CheckMSg(false, "�����ʽ������Ҫ��,������������ַ�����д��ĸ������");
            }

            return new CheckMSg(true, "����ͨ��");
        }

        /// <summary>
        /// �绰���
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
                rsm.Data = new CheckMSg(false, "�绰������Ч��");
                return BadRequest(rsm);
            }

            var user = _context.AuthUsers.FirstOrDefault(e => e.PhoneNo == phoneNo);
            if (user != null)
            {
                rsm.Data = new CheckMSg(false, "�õ绰�����԰������˺�");
                return Ok(rsm);
            }

            rsm.Data = new CheckMSg(true, "����");
            return Ok(rsm);
        }

        /// <summary>
        /// �ʼ����
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
                rsm.Data = new CheckMSg(false, "�ʼ���ʽ��Ч");
                return Ok(rsm);
            }
            var user = _context.AuthUsers.FirstOrDefault(e => e.Email == email);
            if (user != null)
            {
                rsm.Data = new CheckMSg(false, "���ʼ��԰������˺�");
                return Ok(rsm);
            }

            rsm.Data = new CheckMSg(true, "����");
            return Ok(rsm);
        }

        /// <summary>
        /// Ĭ���ض����ַ
        /// </summary>
        /// <returns></returns>
        // ֱ����/��ͷ��·�ɣ��ᵱ�ɾ���·����
        [HttpGet("/Account/Login")]
        [AllowAnonymous]
        public IActionResult Login()
        {
            return Unauthorized("���¼�����ԣ�");
        }


        /// <summary>
        /// ��¼
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
                return Unauthorized("�û����δͨ����֤��");
            }

            var UA = Request.Headers.UserAgent[0];

            var ua = HashUtil.Md5(UA ?? "unkownAgent");

            UserRole? userRole = _context.UserRoles.Include(o => o.Role).FirstOrDefault(e => e.UserId == user.Id);
            if (userRole == null)
            {
                return Unauthorized("�û����δͨ����֤��");
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
        /// ��ѯ�Ƿ���defaultȨ��
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
        /// ע��
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
        /// ˢ��token
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
        /// ע���˺�
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
                return Ok("�ѳɹ�����ע�����룬�ɹ����ͨ���ʼ�֪ͨ��");
            }
            return BadRequest("�������");
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
