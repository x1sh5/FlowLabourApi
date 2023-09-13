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


        [HttpGet("count")]
        [AllowAnonymous]
        public int Count()
        {
            return _context.References.Count();
        }

        // GET: api/<ReferenceController>
        [HttpGet("gets")]
        public IEnumerable<Reference> Gets()
        {
            return _context.References.ToList();
        }

        // GET api/<ReferenceController>/5
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

        // POST api/<ReferenceController>
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

        // PUT api/<ReferenceController>/5
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

        [HttpGet("history/{referId}")]
        public ActionResult<ReferEdit> Histroy([Required]int referId)
        {
            var r = _context.References.Find(referId);
            if (r!=null)
            {
                var e = _context.ReferEdits
                    .Where(e => e.ReferId == referId).ToList();
                return Ok(e);
            }
            return NotFound();
        }

        // DELETE api/<ReferenceController>/5
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
