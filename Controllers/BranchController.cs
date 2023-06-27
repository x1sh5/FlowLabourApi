using System.Collections.Generic;
using System.Linq;
using FlowLabourApi.Models;
using FlowLabourApi.Models.context;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FlowLabourApi.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class BranchController : ControllerBase
    {
        private readonly XiangxpContext _context;

        public BranchController(XiangxpContext context)
        {
            _context = context;
        }

        // GET: api/Branch
        /// <summary>
        /// 获取所有部门
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult<IEnumerable<Branch>> GetBranches()
        {
            return _context.Branches.ToList();
        }

        // GET: api/Branch/5
        /// <summary>
        /// 根据ID，获取相应部门
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public ActionResult<Branch> GetBranch(int id)
        {
            var branch = _context.Branches.Find(id);

            if (branch == null)
            {
                return NotFound();
            }

            return branch;
        }

        // POST: api/Branch
        /// <summary>
        /// 添加新部门
        /// </summary>
        /// <param name="branch"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult<Branch> PostBranch(Branch branch)
        {
            _context.Branches.Add(branch);
            _context.SaveChanges();

            return CreatedAtAction(nameof(GetBranch), new { id = branch.Id }, branch);
        }

        // PUT: api/Branch/5
        /// <summary>
        /// 根据ID，修改相应的部门
        /// </summary>
        /// <param name="id"></param>
        /// <param name="branch"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public IActionResult PutBranch(int id, Branch branch)
        {
            if (id != branch.Id)
            {
                return BadRequest();
            }

            _context.Entry(branch).State = EntityState.Modified;
            _context.SaveChanges();

            return NoContent();
        }

        // DELETE: api/Branch/5
        /// <summary>
        /// 根据ID，删除部门
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public IActionResult DeleteBranch(int id)
        {
            var branch = _context.Branches.Find(id);

            if (branch == null)
            {
                return NotFound();
            }

            _context.Branches.Remove(branch);
            _context.SaveChanges();

            return NoContent();
        }
    }
}
