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
    public class InformationController : ControllerBase
    {
        private readonly XiangxpContext _context;

        public InformationController(XiangxpContext context)
        {
            _context = context;
        }

        /// <summary>
        /// 获取所有部门
        /// </summary>
        /// <returns></returns>
        [HttpGet("branchs")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<Branch>>> GetBranchs()
        {
            return await _context.Branches.ToListAsync();
        }

        /// <summary>
        /// 获取游客能见的所有任务类型
        /// </summary>
        /// <returns></returns>
        [HttpGet("customtypes")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<Assignmenttype>>> GetAssignmentTypes()
        {
            return await _context.Assignmenttypes.Where(e=>e.Level==1).ToListAsync();
        }
    }
}
