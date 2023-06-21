using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FlowLabourApi.Models;
using FlowLabourApi.Models.context;
using FlowLabourApi.ViewModels;
using FlowLabourApi.Utils;

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
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AuthUser>>> GetAuthUsers()
        {
            return await _context.AuthUsers.ToListAsync();
        }

        // GET: api/AuthUser/{id}
        /// <summary>
        /// 获取所有用户信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
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
        /// <summary>
        /// 根据ID获取用户信息
        /// </summary>
        /// <param name="id"></param>
        /// <param name="authUserView"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAuthUser(int id, AuthUserView authUserView)
        {
            if (id != authUserView.Id)
            {
                return BadRequest();
            }
            AuthUser authUser = new AuthUser
            {
                Id = id,
                Username = authUserView.Username,
                Passwordhash = HashUtil.GetHash(authUserView.Password),
                PhoneNo = authUserView.PhoneNo,
                Email = authUserView.Email,
                DateJoined = authUserView.DateJoined,
            };
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
        //also use to register
        /// <summary>
        /// 添加用户信息
        /// </summary>
        /// <param name="authUserView"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult<AuthUser>> PostAuthUser(AuthUserView authUserView)
        {
            AuthUser authUser = new AuthUser
            {
                Username = authUserView.Username,
                Passwordhash = HashUtil.GetHash(authUserView.Password),
                PhoneNo = authUserView.PhoneNo,
                Email = authUserView.Email,
                DateJoined = authUserView.DateJoined,
            };
            _context.AuthUsers.Add(authUser);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetAuthUser), new { id = authUser.Id }, authUser);
        }

        // DELETE: api/AuthUser/{id}
        /// <summary>
        /// 根据ID删除用户信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
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
