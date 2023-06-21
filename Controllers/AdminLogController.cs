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
    /// <summary>
    /// 操作日志记录
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class AdminLogController : ControllerBase
    {
        private readonly XiangxpContext _context;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        public AdminLogController(XiangxpContext context)
        {
            _context = context;
        }

        // GET: api/AdminLog
        /// <summary>
        /// 获取所有记录
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AdminLog>>> GetAdminLog()
        {
            return await _context.AdminLogs.ToListAsync();
        }

        // GET: api/AdminLog/5
        /// <summary>
        /// 根据ID，获取相应记录
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<AdminLog>> GetAdminLog(int id)
        {
            var adminLog = await _context.AdminLogs.FindAsync(id);

            if (adminLog == null)
            {
                return NotFound();
            }

            return adminLog;
        }

        // PUT: api/AdminLog/5
        /// <summary>
        /// 根据ID，修改记录
        /// </summary>
        /// <param name="id"></param>
        /// <param name="adminLogView"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAdminLog(int id, AdminLogView adminLogView)
        {
            if (id != adminLogView.Id)
            {
                return BadRequest();
            }
            AdminLog adminLog = new AdminLog
            {
                User = _context.AuthUsers.Find(adminLogView.UserId),
                ActionType = adminLogView.ActionType,
                Time = adminLogView.Time,
                Describe = adminLogView.Describe
            };

            _context.Entry(adminLog).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AdminLogExists(id))
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

        // POST: api/AdminLog
        /// <summary>
        /// 添加操作记录
        /// </summary>
        /// <param name="adminLogView"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult<AdminLog>> PostAdminLog(AdminLogView adminLogView)
        {
            AdminLog adminLog = new AdminLog
            {
                User = _context.AuthUsers.Find(adminLogView.UserId),
                ActionType = adminLogView.ActionType,
                Time = adminLogView.Time,
                Describe = adminLogView.Describe
            };
            _context.AdminLogs.Add(adminLog);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetAdminLog), new { id = adminLog.Id }, adminLog);
        }

        // DELETE: api/AdminLog/5
        /// <summary>
        /// 根据ID，删除操作记录
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAdminLog(int id)
        {
            var adminLog = await _context.AdminLogs.FindAsync(id);
            if (adminLog == null)
            {
                return NotFound();
            }

            _context.AdminLogs.Remove(adminLog);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool AdminLogExists(int id)
        {
            return _context.AdminLogs.Any(e => e.Id == id);
        }
    }
}
