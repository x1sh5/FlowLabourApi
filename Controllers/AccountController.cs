using FlowLabourApi.Models.context;
using FlowLabourApi.Utils;
using FlowLabourApi.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
        /// ×¢²á
        /// </summary>
        /// <param name="register"></param>
        /// <returns></returns>
        [HttpPost("register")]
        public IActionResult Register([FromBody] RegisterView register)
        {
            // code to handle registration
            return Ok();
        }

        /// <summary>
        /// µÇÂ¼
        /// </summary>
        /// <param name="login"></param>
        /// <returns></returns>
        [HttpPost("login")]
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
            var user = _context.AuthUsers.Find(login.UserName, HashUtil.GetHash(login.Password));
            if(user == null)
            {
                return Unauthorized();
            }
            // code to handle login
            return Ok();
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
            return Ok();
        }
    }
}
