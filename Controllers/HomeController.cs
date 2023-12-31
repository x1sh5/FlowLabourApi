﻿using FlowLabourApi.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace FlowLabourApi.Controllers
{
    /// <summary>
    /// 聊天界面
    /// </summary>
    [Controller]
    [Route("[controller]")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }
        [HttpGet("chat")]
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }
        [HttpGet("test")]
        public IActionResult Test()
        {
            return Content("test test abcd");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}