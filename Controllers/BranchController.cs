using System.Collections.Generic;
using System.Linq;
using FlowLabourApi.Models;
using FlowLabourApi.Models.context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FlowLabourApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BranchController : ControllerBase
    {
        private readonly XiangxpContext _context;

        public BranchController(XiangxpContext context)
        {
            _context = context;
        }

        // GET: api/Branch
        [HttpGet]
        public ActionResult<IEnumerable<Branch>> GetBranches()
        {
            return _context.Branches.ToList();
        }

        // GET: api/Branch/5
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
        /// 
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
