using FlowLabourApi.Config;
using FlowLabourApi.Models;
using FlowLabourApi.Models.context;
using FlowLabourApi.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net.Security;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Security.Policy;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using FlowLabourApi.Utils;
using System.Xml.Linq;
using System.Net.Mail;

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
            return await _context.Identityinfos.Take(10).ToListAsync();
        }

        /// <summary>
        /// 查询是否已经验证通过
        /// </summary>
        /// <returns></returns>
        [HttpGet("validate")]
        public async Task<ActionResult> Validate()
        {
            var userid = User.Claims.FirstOrDefault(c => c.Type == JwtClaimTypes.IdClaim).Value;
            IdentityInfo? i = _context.Identityinfos
                .Where(x => x.AuthId == int.Parse(userid))
                .FirstOrDefault();
            return i?.Checked == 1 ? Ok("已经验证通过。") : BadRequest("身份验证失败！");
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

        /// <summary>
        /// 发送邮箱验证码
        /// </summary>
        /// <returns></returns>
        [HttpGet("mailcode/send")]
        public async Task<ActionResult> SendMailCode(string mail)
        {
            var code = CodeUtil.GenerateRandomCode();
            var userid = User.Claims.FirstOrDefault(c => c.Type == JwtClaimTypes.IdClaim).Value;
            
            
            var codeinfo = new EmailCode()
            {
                AuthId = int.Parse(userid),
                Email = mail,
                Code = code,
            };
            _context.EmailCodes.Add(codeinfo);
            _context.SaveChanges();
            var smt = new SmtpClient
            {
                Host = "smtp.163.com",
                Port = 25,
                EnableSsl = false,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential("","")
            };
            smt.SendCompleted += (s, e) =>
            {
                smt.Dispose();
            };
            smt.SendAsync("","" , "", "验证码", $"您的验证码是：{code}，请在5分钟内输入。");
            
            return Ok();
        }

        /// <summary>
        /// 邮箱验证码确认
        /// </summary>
        /// <param name="code">验证码</param>
        /// <returns></returns>
        [HttpPost("mailcode/confirm")]
        public async Task<ActionResult<IdentityInfo>> MailCodeConfirm(string code)
        {
            var userid = User.Claims.FirstOrDefault(c => c.Type == JwtClaimTypes.IdClaim).Value;
            var c = _context.EmailCodes.Where(x => x.AuthId == int.Parse(userid)).FirstOrDefault();
            if (c?.Code == code)
            {
                return Ok();
            }
            return BadRequest();
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

        /// <summary>
        /// 身份验证
        /// </summary>
        /// <param name="posimg">正面照地址</param>
        /// <param name="name">真实姓名</param>
        /// <parm name="cardNo">身份证号码</parm>
        /// <returns></returns>
        [HttpPost("check")]
        public async Task<IActionResult> Check([Required] string posimg, 
            [Required]string name, [Required]string cardNo)
        {
            var cardNoValid = IdentityValidateUtil.ValidateIdentityNo(cardNo);
            if (!cardNoValid)
            {
                return BadRequest("不合法的身份证号码。");
            }
            var exist = await _context.Identityinfos
                .Where(x => x.IdentityNo == cardNo && x.Checked==1)
                .FirstOrDefaultAsync();
            if(exist!=null)
            {
                return BadRequest("该身份证号码绑定到他人。");
            }

            var userid = User.Claims.FirstOrDefault(c => c.Type == JwtClaimTypes.IdClaim).Value;
            IdentityInfo? i = _context.Identityinfos
                .Where(x=> x.AuthId==int.Parse(userid)&&x.IdentityNo==cardNo&&x.Realname==name)
                .FirstOrDefault();
            if (i != null)
            {
                if(i.Checked == 1){
                    return Ok("已经验证通过。");
                }
                else
                {
                    return BadRequest("身份验证失败！");
                }
            }
            else
            {
                IdentityInfo identityinfo = new IdentityInfo()
                {
                    Realname = name,
                    IdentityNo = cardNo,
                    AuthId = int.Parse(userid),
                    Posimg = posimg,
                };
                var check = await IdcardCheck.Check(name, cardNo);
                if(check.Ok)
                {
                    identityinfo.TaskId = check.TaskId;
                    identityinfo.Checked = 1;
                    identityinfo.Checkeddate = DateTime.Now;
                    _context.Identityinfos.Add(identityinfo);
                    AuthUser? user = await _context.AuthUsers.FindAsync(int.Parse(userid));
                    user!.IsActive = true;
                    await _context.SaveChangesAsync();
                    return Ok("验证完成。");
                }
                else
                {
                    identityinfo.Checked = 0;
                    await _context.SaveChangesAsync();
                    return BadRequest(check.Reason);
                }

            }
            
        }

        
    }
}
