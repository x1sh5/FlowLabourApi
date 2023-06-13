using Microsoft.AspNetCore.Mvc;
using System.Linq;
using FlowLabourApi.Models;
using FlowLabourApi.Models.context;
using Microsoft.EntityFrameworkCore;

namespace FlowLabourApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthUserGroupController : ControllerBase
    {
        private readonly XiangxpContext _context;

        public AuthUserGroupController(XiangxpContext context)
        {
            _context = context;
        }

        // GET: api/AuthUserGroup
        [HttpGet]
        public IActionResult GetAuthUserGroups()
        {
            return Ok(_context.AuthUserGroups.ToList());
        }

        // GET: api/AuthUserGroup/5
        [HttpGet("{id}")]
        public IActionResult GetAuthUserGroup(int id)
        {
            var authUserGroup = _context.AuthUserGroups.Find(id);

            if (authUserGroup == null)
            {
                return NotFound();
            }

            return Ok(authUserGroup);
        }

        // POST: api/AuthUserGroup
        [HttpPost]
        public IActionResult PostAuthUserGroup(AuthUserGroup authUserGroup)
        {
            _context.AuthUserGroups.Add(authUserGroup);
            _context.SaveChanges();

            return CreatedAtAction(nameof(GetAuthUserGroup), new { id = authUserGroup.Id }, authUserGroup);
        }

        // PUT: api/AuthUserGroup/5
        [HttpPut("{id}")]
        public IActionResult PutAuthUserGroup(int id, AuthUserGroup authUserGroup)
        {
            if (id != authUserGroup.Id)
            {
                return BadRequest();
            }

            _context.Entry(authUserGroup).State = EntityState.Modified;
            _context.SaveChanges();

            return NoContent();
        }

        // DELETE: api/AuthUserGroup/5
        [HttpDelete("{id}")]
        public IActionResult DeleteAuthUserGroup(int id)
        {
            var authUserGroup = _context.AuthUserGroups.Find(id);

            if (authUserGroup == null)
            {
                return NotFound();
            }

            _context.AuthUserGroups.Remove(authUserGroup);
            _context.SaveChanges();

            return NoContent();
        }
    }
}
