using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace FlowLabourApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TestController : ControllerBase
    {
        [HttpGet("login/cookieset")]
        public IActionResult Get()
        {
            Response.Headers.AccessControlExposeHeaders.Append("Set-Cookie");
            Response.Cookies.Append("loginrequare", "login need"+DateTime.Now);
            return Ok("Hello World!");
        }

        [HttpGet("cookieset")]
        [AllowAnonymous]
        public IActionResult Get2()
        {
            //Response.Headers.AccessControlExposeHeaders.Append("Set-Cookie");
            Response.Headers.AccessControlExposeHeaders = new string[]{ "Set-Cookie", "Server", "Content-Type" };
            Response.Cookies.Append("nologin", "login not need" + DateTime.Now,new CookieOptions
            {
                SameSite = SameSiteMode.None,
                Secure = true,
            });
            return Ok("Hello World!");
        }

    }
}
