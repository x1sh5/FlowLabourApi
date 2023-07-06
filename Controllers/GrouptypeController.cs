using FlowLabourApi.Models;
using FlowLabourApi.Models.context;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FlowLabourApi.Controllers
{
    /// <summary>
    /// ����Ϣ��rest API
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
        /// ��ȡ������������Ϣ
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Grouptype>>> GetGrouptypes()
        {
            return await _context.Grouptypes.ToListAsync();
        }

        // GET: api/Grouptype/5
        /// <summary>
        /// ����ID����ȡ��������Ϣ
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
        /// ����ID���޸�������
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
        /// ���������
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
        /// ������ID��ɾ��������
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
