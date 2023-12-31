﻿using FlowLabourApi.Models;
using FlowLabourApi.Models.context;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace FlowLabourApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InformationController : ControllerBase
    {
        private readonly FlowContext _context;

        public InformationController(FlowContext context)
        {
            _context = context;
        }

        /// <summary>
        /// 获取所有部门
        /// </summary>
        /// <returns></returns>
        [HttpGet("branchs")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<Branch>>> GetBranchs()
        {
            var reslut =  await _context.Branches.AsNoTracking().ToListAsync();
            return reslut;
        }

        /// <summary>
        /// 获取游客能见的所有任务类型
        /// </summary>
        /// <returns></returns>
        [HttpGet("customtypes")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<Branch>>> GetAssignmentTypes()
        {
            var reslut = await _context.Branches.AsNoTracking().ToListAsync();
            return reslut;
        }

        /// <summary>
        /// 热搜任务
        /// </summary>
        /// <returns></returns>
        [HttpGet("popular")]
        public async Task<ActionResult<IEnumerable<string>>> GetPopulars()
        {
            var reslut =  _context.Searchs
                .GroupBy(x => x.Word)
                .OrderByDescending(group => group.Count())
                .Select(k => k.Key)
                .Take(10).ToList();
            return reslut;
        }
    }
}
