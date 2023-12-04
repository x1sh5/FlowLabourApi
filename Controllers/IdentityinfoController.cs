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
        public async Task<ActionResult<IEnumerable<Models.IdentityInfo>>> GetIdentityinfos()
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
            Models.IdentityInfo? i = _context.Identityinfos
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
        public async Task<ActionResult<Models.IdentityInfo>> GetIdentityinfo(int id)
        {
            var identityinfo = await _context.Identityinfos.FirstOrDefaultAsync(i => i.Id == id);

            if (identityinfo == null)
            {
                return NotFound();
            }

            return identityinfo;
        }

        /// <summary>
        /// 获取当前用户的绑定邮箱
        /// </summary>
        /// <returns></returns>
        [HttpGet("email")]
        public async Task<ActionResult> GetMail()
        {
            var userid = User.Claims.FirstOrDefault(c => c.Type == JwtClaimTypes.IdClaim).Value;
            var user = await _context.AuthUsers.FindAsync(int.Parse(userid));
            return Ok(user==null?"":user.Email);
        }

        /// <summary>
        /// 发送邮箱验证码
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        [HttpGet("emailcode/send")]
        public async Task<ActionResult> SendMailCode(string email)
        {
            var isok = IdentityValidateUtil.ValidateEmailFormat(email);
            if(!isok)
            {
                return BadRequest("不合法的邮箱地址。");
            }

            var user = _context.AuthUsers.FirstOrDefault(e => e.Email == email);
            if (user != null)
            {
                return BadRequest("该邮件以绑定其他账号");
            }

            var code = CodeUtil.GenerateRandomCode();
            var userid = User.Claims.FirstOrDefault(c => c.Type == JwtClaimTypes.IdClaim).Value;
            
            
            var codeinfo = new EmailCode()
            {
                AuthId = int.Parse(userid),
                Email = email,
                Code = code,
            };
            _context.EmailCodes.Add(codeinfo);
            _context.SaveChanges();
            var smtpClient = new SmtpClient("smtp.qq.com")
            {
                Port = 587,
                Credentials = new NetworkCredential("2541613004@qq.com", "cqtsytxbcfqxecgb"),//
                EnableSsl = true,
            };
            var mailMessage = new MailMessage
            {
                From = new MailAddress("2541613004@qq.com", "流沙任务系统", System.Text.Encoding.UTF8),
                Subject = "安全验证码",
                SubjectEncoding = System.Text.Encoding.UTF8,
                Body = $"您的验证码是：{code}，请在5分钟内输入。",
                BodyEncoding = System.Text.Encoding.UTF8,
                IsBodyHtml = false,
            };
            mailMessage.To.Add(email);
            try
            {
                smtpClient.Send(mailMessage);
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
            
            return NoContent();
        }

        /// <summary>
        /// 邮箱验证码确认
        /// </summary>
        /// <param name="code">验证码</param>
        /// <returns></returns>
        [HttpPost("emailcode/confirm")]
        public async Task<ActionResult<Models.IdentityInfo>> MailCodeConfirm(string? code)
        {
            var userid = User.Claims.FirstOrDefault(c => c.Type == JwtClaimTypes.IdClaim).Value;
            var c = _context.EmailCodes.Where(x => x.AuthId == int.Parse(userid)
            &&x.Code==code).FirstOrDefault();

            if (c!=null)
            {
                if (c.IsExpired)
                {
                    BadRequest("验证码过期");
                }
                var user = await _context.AuthUsers.FindAsync(int.Parse(userid));
                user!.Email = c.Email;
                _context.EmailCodes.Remove(c);
                await _context.SaveChangesAsync();
                return Ok("绑定成功");
            }
            return BadRequest("验证码错误");
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
            Models.IdentityInfo identityinfo = new IdentityInfo()
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
