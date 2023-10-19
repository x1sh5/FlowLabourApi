using FlowLabourApi.Config;
using FlowLabourApi.Models;
using FlowLabourApi.Models.context;
using FlowLabourApi.ViewModels;
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
    public class IdentityInfoController : ControllerBase
    {
        private readonly FlowContext _context;

        public IdentityInfoController(FlowContext context)
        {
            _context = context;
        }

        // GET: api/IdentityInfo
        /// <summary>
        /// 获取所有身份信息
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<IdentityInfo>>> GetIdentityinfos()
        {
            return await _context.Identityinfos.ToListAsync();
        }

        // GET: api/IdentityInfo/5
        /// <summary>
        /// 根据ID，获取相应的身份信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<IdentityInfo>> GetIdentityinfo(int id)
        {
            var identityinfo = await _context.Identityinfos.FirstOrDefaultAsync(i => i.Id == id);

            if (identityinfo == null)
            {
                return NotFound();
            }

            return identityinfo;
        }

        // PUT: api/IdentityInfo/5
        /// <summary>
        /// 根据ID，修改身份信息
        /// </summary>
        /// <param name="id"></param>
        /// <param name="identityinfoView"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> PutIdentityinfo(int id, IdentityinfoView identityinfoView)
        {
            if (id != identityinfoView.Id)
            {
                return BadRequest();
            }
            IdentityInfo identityinfo = new IdentityInfo()
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

        // POST: api/IdentityInfo
        /// <summary>
        /// 添加身份信息
        /// </summary>
        /// <param name="identityinfoRegister"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult<IdentityInfo>> PostIdentityinfo(IdentityinfoRegister identityinfoRegister)
        {
            if (!identityinfoRegister.IsValidate)
            {
                return BadRequest("不合法的身份信息。");
            }
            IdentityInfo identityinfo = new IdentityInfo()
            {
                Realname = identityinfoRegister.Realname,
                IdentityNo = identityinfoRegister.IdentityNo,
            };
            _context.Identityinfos.Add(identityinfo);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetIdentityinfo), new { id = identityinfo.Id }, identityinfo);
        }

        // DELETE: api/IdentityInfo/5
        /// <summary>
        /// 根据ID，删除身份信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
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

        [NonAction]
        private bool IdentityinfoExists(int id)
        {
            return _context.Identityinfos.Any(e => e.Id == id);
        }

        [HttpPost("check")]
        public async Task<IActionResult> Check(string posimg, string negimg)
        {
            var userid = User.Claims.FirstOrDefault(c => c.Type == JwtClaimTypes.IdClaim).Value;
            var i = _context.Identityinfos.Where(x=> x.AuthId==int.Parse(userid)).FirstOrDefault();
            if (i == null)
            {
                return BadRequest("没有找到对应的身份信息。");
            }
            i.Posimg = posimg;
            i.Negimg = negimg;
            _context.SaveChanges();
            return Ok("上传信息成功，请等待验证完成。");
        }
    }
}
