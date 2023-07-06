using FlowLabourApi.Models;
using FlowLabourApi.Models.context;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FlowLabourApi.Controllers
{
    /// <summary>
    /// 组信息的rest API
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class GrouptypeController : ControllerBase
    {
        private readonly XiangxpContext _context;

        public GrouptypeController(XiangxpContext context)
        {
            _context = context;
        }

        // GET: api/Grouptype
        /// <summary>
        /// 获取所有组类型信息
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Grouptype>>> GetGrouptypes()
        {
            return await _context.Grouptypes.ToListAsync();
        }

        // GET: api/Grouptype/5
        /// <summary>
        /// 根据ID，获取组类型信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<Grouptype>> GetGrouptype(int id)
        {
            var grouptype = await _context.Grouptypes.FindAsync(id);

            if (grouptype == null)
            {
                return NotFound();
            }

            return grouptype;
        }

        // PUT: api/Grouptype/5
        /// <summary>
        /// 根据ID，修改组类型
        /// </summary>
        /// <param name="id"></param>
        /// <param name="grouptype"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> PutGrouptype(int id, Grouptype grouptype)
        {
            if (id != grouptype.Id)
            {
                return BadRequest();
            }

            _context.Entry(grouptype).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!GrouptypeExists(id))
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

        // POST: api/Grouptype
        /// <summary>
        /// 添加组类型
        /// </summary>
        /// <param name="grouptype"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult<Grouptype>> PostGrouptype(Grouptype grouptype)
        {
            _context.Grouptypes.Add(grouptype);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetGrouptype", new { id = grouptype.Id }, grouptype);
        }

        // DELETE: api/Grouptype/5
        /// <summary>
        /// 根据组ID，删除组类型
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGrouptype(int id)
        {
            var grouptype = await _context.Grouptypes.FindAsync(id);
            if (grouptype == null)
            {
                return NotFound();
            }

            _context.Grouptypes.Remove(grouptype);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool GrouptypeExists(int id)
        {
            return _context.Grouptypes.Any(e => e.Id == id);
        }
    }
}
