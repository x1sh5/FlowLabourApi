using FlowLabourApi.Config;
using FlowLabourApi.Models;
using FlowLabourApi.Models.context;
using FlowLabourApi.Models.state;
using FlowLabourApi.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace FlowLabourApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TaskRequestController : ControllerBase
    {
        private readonly FlowContext _context;
        private readonly ILogger<TaskRequestController> _logger;

        public TaskRequestController(FlowContext context, ILogger<TaskRequestController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: api/<TaskRequestController>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TaskRequest>>> Gets()
        {
            return await _context.TaskRequests.ToListAsync();
        }

        /// <summary>
        /// 我的申请
        /// </summary>
        /// <returns></returns>
        [HttpGet("myapply")]
        public async Task<ActionResult<TaskRequest>> MyApply()
        {
            var userid = User.Claims.FirstOrDefault(User => User.Type == JwtClaimTypes.IdClaim).Value;
            var r = _context.TaskRequests.Where(x => x.UserId == int.Parse(userid)).ToList();
            if (r == null)
            {
                return NotFound();
            }
            return Ok(r);
        }

        /// <summary>
        /// 申请我的任务
        /// </summary>
        /// <returns></returns>
        [HttpGet("applytome")]
        public async Task<ActionResult<TaskRequest>> ApplyToMe()
        {
            var userid = User.Claims.FirstOrDefault(User => User.Type == JwtClaimTypes.IdClaim).Value;
            var asignments = _context.Assignments
                .Where(x => x.UserId == int.Parse(userid))
                .Select(x=>x.Id).ToList();
            var r = _context.TaskRequests.Where(x => asignments.Contains(x.TaskId)).ToList();
            if (r == null)
            {
                return NotFound();
            }
            return Ok(r);
        }

        // GET api/<TaskRequestController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TaskRequest>> Get(int id)
        {
            var r = _context.TaskRequests.SingleOrDefault(x => x.Id == id);
            if(r == null)
            {
                return NotFound();
            }
            return Ok(r);
        }

        // POST api/<TaskRequestController>
        [HttpPost]
        public async Task<ActionResult> Post([FromBody] TaskRequest value)
        {
            var userid = User.Claims.FirstOrDefault(User => User.Type == JwtClaimTypes.IdClaim).Value;

            var aus = _context.Assignmentusers
            .Where(x => x.UserId == int.Parse(userid))
            .Include(o => o.Assignment).ToList();

            var c = aus.Select(o => o.Assignment.Status == (sbyte)TaskState.WaitForAccept).Count();
            if (c >= 1)
            {
                return BadRequest("有待完成的任务，请完成后再接取新任务。");
            }

            var entity1 = _context.TaskRequests
                .SingleOrDefault(x => x.UserId == int.Parse(userid) && x.TaskId == value.TaskId);
            if(entity1 != null)
            {
                if (entity1.Agree == 0)
                {
                    if(DateTime.Now - entity1.RequestDate < TimeSpan.FromHours(8))
                    {
                           return BadRequest("您最近已经申请过了，请稍后再申请！");
                    }
                    else
                    {
                        entity1.Agree = 2;
                        entity1.RequestDate = DateTime.Now;
                        try
                        {
                            _context.TaskRequests.Update(entity1);
                            _context.SaveChanges();
                            return Ok("申请已成功递交，请等待！");
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, $"update TaskRequest:{entity1} err");
                            return BadRequest("error");
                        }
                    }
                }

                return BadRequest("您已经申请过了，请勿重复申请！");
            }

            var e = new TaskRequest
            {
                UserId = int.Parse(userid),
                TaskId = value.TaskId,
                Agree = 2,
                Title = value.Title,
                TypeId = value.TypeId,
                RequestDate = DateTime.Now,
                Comment = value.Comment,
            };
            try
            {
                _context.TaskRequests.Add(e);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"add TaskRequest:{e} err");
                return BadRequest("申请任务出错！");
            }
            return Ok("申请已成功递交，请等待！");
        }

        // PUT api/<TaskRequestController>/5
        [HttpPut("agree/{id}")]
        public async Task<ActionResult> Agree(int id, [FromBody] TaskRequest value)
        {
            if(id != value.Id)
            {
                return BadRequest();
            }
            var r = _context.TaskRequests.SingleOrDefault(x => x.Id == id);
            if (r == null)
            {
                return NotFound();
            }
            r.Agree = 1;
            r.Comment = value.Comment;
            r.AgreeDate = DateTime.Now;
            try
            {
                _context.TaskRequests.Update(r);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"update TaskRequest:{r} err");
                return BadRequest("状态修改出错");
            }
            return Ok("修改成功");
        }

        [HttpPut("disagree/{id}")]
        public async Task<ActionResult> Disagree(int id, [FromBody] TaskRequest value)
        {
            if (id != value.Id)
            {
                return BadRequest();
            }
            var r = _context.TaskRequests.SingleOrDefault(x => x.Id == id);
            if (r == null)
            {
                return NotFound();
            }
            r.Agree = 0;
            r.Comment = value.Comment;
            r.AgreeDate = DateTime.Now;
            try
            {
                _context.TaskRequests.Update(r);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"update TaskRequest:{r} err");
                return BadRequest("状态修改出错");
            }
            return Ok("修改成功");
        }

        // DELETE api/<TaskRequestController>/5
        [HttpDelete("{id}")]
        public ActionResult Delete(int id)
        {
            var r = _context.TaskRequests.SingleOrDefault(x => x.Id == id);
            if (r == null)
            {
                return NotFound();
            }
            try
            {
                _context.TaskRequests.Remove(r);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"delete TaskRequest:{r} err");
                return BadRequest();
            }

            return Ok("删除成功");
        }
    }
}
