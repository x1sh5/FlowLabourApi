using FlowLabourApi.Config;
using FlowLabourApi.Models;
using FlowLabourApi.Models.context;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace FlowLabourApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AssignmentUserController : ControllerBase
    {
        private readonly XiangxpContext _xiangxpContext;
        private readonly UserManager<AuthUser> _userManager;

        public AssignmentUserController(XiangxpContext xiangxpContext, UserManager<AuthUser> userManager)
        {
            _xiangxpContext = xiangxpContext;
            _userManager = userManager;
        }

        [HttpGet("holds")]
        public async Task<ActionResult<IEnumerable<Assignment>>> GetAssignmentUsers()
        {
            var id = User.Claims.FirstOrDefault(User => User.Type == JwtClaimTypes.IdClaim).Value;
            //var user = await _userManager.FindByIdAsync(id);
            List<Assignment>? assignments = new List<Assignment>();
            var x = await _xiangxpContext.Assignmentusers
                .Where(e => e.UserId == Convert.ToInt32(id))
                .ToListAsync();
            foreach(var e in x)
            {
                var a = _xiangxpContext.Assignments.Find(e.AssignmentId);
                if(a != null)
                {
                    assignments.Add(a);
                }

            }

            return assignments;
        }
    }
}
