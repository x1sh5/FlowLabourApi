using FlowLabourApi.Config;
using FlowLabourApi.Models;
using FlowLabourApi.Models.context;
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
            var e = new TaskRequest
            {
                UserId = int.Parse(userid),
                TaskId = value.TaskId,
                Agree = 0,
                RequestDate = DateTime.Now,
                Comment = value.Comment,
            };
            try
            {
                _context.TaskRequests.Add(e);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"add TaskRequest:{e} err");
            }
            return Ok("申请成功，请等待！");
        }

        // PUT api/<TaskRequestController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<TaskRequestController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
