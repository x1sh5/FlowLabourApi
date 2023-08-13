﻿using FlowLabourApi.Models.context;
using FlowLabourApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using FlowLabourApi.ViewModels;
using Microsoft.AspNetCore.Authorization;
using FlowLabourApi.Config;

namespace FlowLabourApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class HistoryController : ControllerBase
    {
        private readonly XiangxpContext _context;

        public HistoryController(XiangxpContext context)
        {
            _context = context;
        }

        /// <summary>
        /// 获取所有历史记录
        /// </summary>
        /// <returns></returns>
        // GET: api/History
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AssignmentView>>> GetHistory([Required]int count, 
            [Required]int offset)
        {
            var id = User.Claims.FirstOrDefault(User => User.Type == JwtClaimTypes.IdClaim).Value;
            var userName = User.Claims.FirstOrDefault(User => User.Type == JwtClaimTypes.NameClaim).Value;
            List<History>?asgms = await _context.Historys.Include(o=>o.Assignment)
                .Where(o=>o.Id>offset && o.UserId == Convert.ToInt32(id)).Take(count).ToListAsync();
            List<AssignmentView> assignments = new List<AssignmentView>();
            foreach (var item in asgms)
            {
                assignments.Add(new AssignmentView
                {
                    Id = item.Assignment.Id,
                    UserId = item.Assignment.UserId,
                    Username = userName,
                    Title = item.Assignment.Title,
                    Branchid = item.Assignment.Branchid,
                    Description = item.Assignment.Description,
                    Typeid = item.Assignment.Typeid,
                    Status = item.Assignment.Status,
                    Finishtime = item.Assignment.Finishtime,
                    Presumedtime = item.Assignment.Presumedtime,
                    Publishtime = item.Assignment.Publishtime,
                    Reward = item.Assignment.Reward,
                    Rewardtype = item.Assignment.Rewardtype,
                    Verify = item.Assignment.Verify,
                });
            }

            return Ok(assignments);
        }

        // GET: api/History/5
        /// <summary>
        /// 获取指定ID的历史记录
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<History>> GetHistory(int id)
        {
            var history = await _context.Historys.FindAsync(id);

            if (history == null)
            {
                return NotFound();
            }

            return history;
        }

        /// <summary>
        /// 生成历史记录
        /// </summary>
        /// <param name="history"></param>
        /// <returns></returns>
        // POST: api/History
        [HttpPost]
        public async Task<ActionResult<History>> PostHistory(HistoryView historyView)
        {
            var history = new History
            {
                AssigmentId = historyView.AssigmentId,
                Time = historyView.Time
            };
            _context.Historys.Add(history);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetHistory", new { id = history.Id }, history);
        }

        /// <summary>
        /// 删除历史记录
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // DELETE: api/History/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<History>> DeleteHistory(int id)
        {
            var history = await _context.Historys.FindAsync(id);
            if (history == null)
            {
                return NotFound();
            }

            _context.Historys.Remove(history);
            await _context.SaveChangesAsync();

            return history;
        }

        private bool HistoryExists(int id)
        {
            return _context.Historys.Any(e => e.Id == id);
        }
    }
}
