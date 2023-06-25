using FlowLabourApi.Models.context;
using FlowLabourApi.Utils;
using FlowLabourApi.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NuGet.Common;

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

        public AccountController(XiangxpContext context)
        {
            _context = context;
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
            // code to handle registration
            return Ok();
        }

        /// <summary>
        /// ��¼��ʾ
        /// </summary>
        /// <param name="login"></param>
        /// <returns></returns>
        [HttpGet("login")]
        //[HttpPost("Login")]
        [AllowAnonymous]
        public IActionResult Login()
        {
            return Unauthorized("���¼�����ԣ�");
        }

        ///// <summary>
        ///// ��¼��ʾ
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
        /// ��¼
        /// </summary>
        /// <param name="login"></param>
        /// <returns></returns>
        [HttpPost("login")]
        //[HttpPost("Login")]
        [AllowAnonymous]
        public IActionResult Login([FromBody] LoginView login)
        {
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
            // code to handle login
            return Ok();
        }

        /// <summary>
        /// ע��
        /// </summary>
        /// <returns></returns>
        [HttpPost("logout")]
        [Authorize]
        public IActionResult Logout()
        {
            // code to handle logout
            return Ok();
        }


    }
}
