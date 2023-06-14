using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FlowLabourApi.Models;
using FlowLabourApi.Models.context;
using FlowLabourApi.ViewModels;

namespace FlowLabourApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IdentityinfoController : ControllerBase
    {
        private readonly XiangxpContext _context;

        public IdentityinfoController(XiangxpContext context)
        {
            _context = context;
        }

        // GET: api/Identityinfo
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Identityinfo>>> GetIdentityinfos()
        {
            return await _context.Identityinfos.ToListAsync();
        }

        // GET: api/Identityinfo/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Identityinfo>> GetIdentityinfo(int id)
        {
            var identityinfo = await _context.Identityinfos.FirstOrDefaultAsync(i=>i.Id==id);

            if (identityinfo == null)
            {
                return NotFound();
            }

            return identityinfo;
        }

        // PUT: api/Identityinfo/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutIdentityinfo(int id, IdentityinfoView identityinfoView)
        {
            if (id != identityinfoView.Id)
            {
                return BadRequest();
            }
            Identityinfo identityinfo = new Identityinfo()
            {
                Id = identityinfoView.Id,
                Realname = identityinfoView.Realname,
                IdentityNo = identityinfoView.IdentityNo,
                Checked = identityinfoView.Checked,
                Checkeddate = identityinfoView.Checkeddate,
            };

            _context.Entry(identityinfo).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!IdentityinfoExists(id))
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

        // POST: api/Identityinfo
        [HttpPost]
        public async Task<ActionResult<Identityinfo>> PostIdentityinfo(IdentityinfoRegister identityinfoRegister)
        {
            if(!identityinfoRegister.IsValidate) {
                return BadRequest("不合法的身份信息。");
            }
            Identityinfo identityinfo = new Identityinfo()
            {
                Realname = identityinfoRegister.Realname,
                IdentityNo = identityinfoRegister.IdentityNo,
            };
            _context.Identityinfos.Add(identityinfo);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetIdentityinfo), new { id = identityinfo.Id }, identityinfo);
        }

        // DELETE: api/Identityinfo/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteIdentityinfo(int id)
        {
            var identityinfo = await _context.Identityinfos.FindAsync(id);
            if (identityinfo == null)
            {
                return NotFound();
            }

            _context.Identityinfos.Remove(identityinfo);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool IdentityinfoExists(int id)
        {
            return _context.Identityinfos.Any(e => e.Id == id);
        }
    }
}
