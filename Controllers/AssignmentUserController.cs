using FlowLabourApi.Config;
using FlowLabourApi.Models;
using FlowLabourApi.Models.context;
using FlowLabourApi.Models.state;
using FlowLabourApi.ViewModels;
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
        private readonly FlowContext _xiangxpContext;

        public AssignmentUserController(FlowContext xiangxpContext)
        {
            _xiangxpContext = xiangxpContext;
        }

        /// <summary>
        /// 持有的任务
        /// </summary>
        /// <returns></returns>
        [HttpGet("holds")]
        public async Task<ActionResult<IEnumerable<Assignment>>> GetAssignmentUsers()
        {
            var id = User.Claims.FirstOrDefault(User => User.Type == JwtClaimTypes.IdClaim).Value;
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

        /// <summary>
        /// 放弃任务
        /// </summary>
        /// <param name="asgid"></param>
        /// <returns></returns>
        [HttpDelete("abandon/{asgid}")]
        public async Task<ActionResult> Abandon(int asgid)
        {
            var id = User.Claims.FirstOrDefault(User => User.Type == JwtClaimTypes.IdClaim).Value;
            var assignmentuser = await _xiangxpContext.Assignmentusers.FindAsync(asgid, Convert.ToInt32(id));
            if (assignmentuser == null)
            {
                return NotFound();
            }
            var assignment = await _xiangxpContext.Assignments.FindAsync(asgid);
            if(assignment != null)
            {
                assignment.Status = (sbyte)TaskState.WaitForAccept;
            }

            _xiangxpContext.Assignmentusers.Remove(assignmentuser);
            await _xiangxpContext.SaveChangesAsync();

            return NoContent();
        }

        /// <summary>
        /// get assignments by status
        /// </summary>
        /// <param name="statuscode"></param>
        /// <returns></returns>
        [HttpGet("status/{statuscode}")]
        public async Task<ActionResult<IEnumerable<AssignmentView>>> GetAssignmentsByStatus(int statuscode)
        {
            var id = User.Claims.FirstOrDefault(User => User.Type == JwtClaimTypes.IdClaim).Value;
            var userName = User.Claims.FirstOrDefault(User => User.Type == JwtClaimTypes.NameClaim).Value;
            List<AssignmentView>? assignments = new List<AssignmentView>();
            var x = await _xiangxpContext.Assignmentusers
                .Include(o=>o.Assignment)
                .Where(e => e.UserId == Convert.ToInt32(id) && e.Assignment.Status == statuscode)
                .ToListAsync();
            foreach (var item in x)
            {
                assignments.Add(new AssignmentView
                {
                    Id = item.Assignment.Id,
                    UserId = item.Assignment.UserId,
                    Username = userName,
                    Title = item.Assignment.Title,
                    Branchid = item.Assignment.Branchid,
                    Description = item.Assignment.Description,
                    TypeId = item.Assignment.TypeId,
                    Status = item.Assignment.Status,
                    Finishtime = item.Assignment.Finishtime,
                    Deadline = item.Assignment.Deadline,
                    Publishtime = item.Assignment.Publishtime,
                    FixedReward = item.Assignment.FixedReward,
                    PercentReward = item.Assignment.PercentReward,
                    Rewardtype = item.Assignment.Rewardtype,
                    Verify = item.Assignment.Verify,
                    CanTake = item.Assignment.CanTake,
                    Main = item.Assignment.Main,
                });
            }
            return Ok(assignments);

        }
    }
}
