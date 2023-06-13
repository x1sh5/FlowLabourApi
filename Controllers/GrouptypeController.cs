using FlowLabourApi.Models;
using FlowLabourApi.Models.context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlowLabourApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GrouptypeController : ControllerBase
    {
        private readonly XiangxpContext _context;

        public GrouptypeController(XiangxpContext context)
        {
            _context = context;
        }

        // GET: api/Grouptype
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Grouptype>>> GetGrouptypes()
        {
            return await _context.Grouptypes.ToListAsync();
        }

        // GET: api/Grouptype/5
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
        [HttpPost]
        public async Task<ActionResult<Grouptype>> PostGrouptype(Grouptype grouptype)
        {
            _context.Grouptypes.Add(grouptype);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetGrouptype", new { id = grouptype.Id }, grouptype);
        }

        // DELETE: api/Grouptype/5
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
