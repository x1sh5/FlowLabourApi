using FlowLabourApi.Config;
using FlowLabourApi.Models;
using FlowLabourApi.Models.context;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

// For more information on enabling Web API for empty projects,
// visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace FlowLabourApi.Controllers
{
    /// <summary>
    /// 审核区间参考
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ReferenceController : ControllerBase
    {
        private readonly FlowContext _context;
        private readonly ILogger<ReferenceController> _logger;

        public ReferenceController(FlowContext context, 
            ILogger<ReferenceController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// 参考区间数量
        /// </summary>
        /// <returns></returns>
        [HttpGet("count")]
        [AllowAnonymous]
        public int Count()
        {
            return _context.References.Count();
        }

        /// <summary>
        /// 获取参考区间
        /// </summary>
        /// <param name="count">数量</param>
        /// <param name="offset">ID偏移量</param>
        /// <returns></returns>
        [HttpGet("gets")]
        public IEnumerable<Reference> Gets([Required]int count, [Required]int offset)
        {
            return _context.References.Where(x=>x.Id>offset).Take(count).ToList();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public ActionResult<Reference> Get(int id)
        {
            var e = _context.References.Find(id);
            if (e!=null)
            {
                return Ok(e);
            }
            return NotFound();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="title">标题</param>
        /// <returns></returns>
        [HttpGet("search/{title}")]
        public ActionResult<Reference> Search(string title)
        {
            var e = _context.References
                .Where(x=>x.Title.Contains(title))
                .Take(10).ToArray();
            if (e != null)
            {
                return Ok(e);
            }
            return NotFound();
        }

        /// <summary>
        /// 新建审核参考区间
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult<Reference> Post([FromBody] Reference value)
        {
            var id = User.Claims
                .FirstOrDefault(c => c.Type == JwtClaimTypes.IdClaim)?.Value;
            var userName = User!.Claims
                .FirstOrDefault(User => User.Type == JwtClaimTypes.NameClaim).Value;

            var has = _context.References
                .FirstOrDefault(r => r.Title == value.Title);
            if (has != null)
            {
                return BadRequest("添加失败！已存在同名的审核区间参考。");
            }

            var r = new Reference()
            {
                Title = value.Title,
                Content = value.Content,
                AuthId = int.Parse(id),
                CreateTime = DateTime.Now,
                UserName = userName,
                Version = 1
            };
            try
            {
                _context.References.Add(r);
                _context.SaveChanges();
                return Ok("添加成功！");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return BadRequest("添加失败！");
            }
        }

        /// <summary>
        /// 修改审核区间参考
        /// </summary>
        /// <param name="id"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public ActionResult<Reference> Put(int id, [FromBody] Reference value)
        {
            var r = _context.References.Find(id);
            if (r!=null)
            {
                var e = new ReferEdit()
                {
                    ReferId = r.Id,
                    Old = r.Content,
                    Change = "",
                    Version = value.Version,
                    UserName = User.Claims
                        .FirstOrDefault(c => c.Type == JwtClaimTypes.NameClaim)?.Value,
                    EditTime = DateTime.Now,
                    Title = value.Title
                };

                r.Title = value.Title;
                r.Content = value.Content;
                r.AuthId = value.AuthId;
                r.CreateTime = DateTime.Now;
                r.Version += 1;

                try
                {
                    _context.ReferEdits.Add(e);
                    _context.References.Update(r);
                    _context.SaveChanges();
                    return Ok("修改成功！");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message);
                    return BadRequest("修改失败！");
                }
            }
            return NotFound();
        }

        /// <summary>
        /// 历史修改记录
        /// </summary>
        /// <param name="referId"></param>
        /// <param name="offset">id偏移量</param>
        /// <param name="count">数量</param>
        /// <returns></returns>
        [HttpGet("history/{referId}")]
        public ActionResult<ReferEdit> Histroy([Required]int referId,
            [Required] int count, [Required] int offset)
        {
            var r = _context.References.Find(referId);
            if (r!=null)
            {
                var e = _context.ReferEdits
                    .Where(e => e.ReferId == referId&&e.Id>offset).Take(count).ToList();
                return Ok(e);
            }
            return NotFound();
        }

        /// <summary>
        /// 删除审核区间参考
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public ActionResult<Reference> Delete(int id)
        {
            var r = _context.References.Find(id);
            try
            {
                _context.References.Remove(r);
                _context.SaveChanges();
                return Ok("删除成功！");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return BadRequest("删除失败！");
            }
        }
    }
}
