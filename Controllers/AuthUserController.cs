using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FlowLabourApi.Models;
using FlowLabourApi.Models.context;

namespace FlowLabourApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthUserController : ControllerBase
    {
        private readonly XiangxpContext _context;

        public AuthUserController(XiangxpContext context)
        {
            _context = context;
        }

        // GET: api/AuthUser
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AuthUser>>> GetAuthUsers()
        {
            return await _context.AuthUsers.ToListAsync();
        }

        // GET: api/AuthUser/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<AuthUser>> GetAuthUser(int id)
        {
            var authUser = await _context.AuthUsers.FindAsync(id);

            if (authUser == null)
            {
                return NotFound();
            }

            return authUser;
        }

        // PUT: api/AuthUser/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAuthUser(int id, AuthUserView authUserView)
        {
            AuthUser authUser = new AuthUser
            {
                Id = id,
                Username = authUserView.Username,
                Passwordhash = authUserView.Passwordhash,
                PhoneNo = authUserView.PhoneNo,
                Email = authUserView.Email,
                DateJoined = authUserView.DateJoined,
                IsActive = authUserView.IsActive,
                LastLogin = authUserView.LastLogin,
                Roles
            };
            if (id != authUser.Id)
            {
                return BadRequest();
            }

            _context.Entry(authUser).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AuthUserExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/AuthUser
        [HttpPost]
        public async Task<ActionResult<AuthUser>> PostAuthUser(AuthUser authUser)
        {
            _context.AuthUsers.Add(authUser);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetAuthUser), new { id = authUser.Id }, authUser);
        }

        // DELETE: api/AuthUser/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAuthUser(int id)
        {
            var authUser = await _context.AuthUsers.FindAsync(id);
            if (authUser == null)
            {
                return NotFound();
            }

            _context.AuthUsers.Remove(authUser);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool AuthUserExists(int id)
        {
            return _context.AuthUsers.Any(e => e.Id == id);
        }
    }
}
