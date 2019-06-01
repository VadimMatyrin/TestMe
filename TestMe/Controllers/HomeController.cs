using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using TestMe.Config;
using TestMe.Data;
using TestMe.Models;
using TestMe.Sevices.Interfaces;

namespace TestMe.Controllers
{
    public class HomeController : Controller
    {
        private readonly ITestingPlatform _testingPlatform;
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IOptions<LoadConfig> _loadConfig;
        public HomeController(ITestingPlatform testingPlatform, UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IOptions<LoadConfig> loadConfig)
        {
            _testingPlatform = testingPlatform;
            _userManager = userManager;
            _signInManager = signInManager;
            _loadConfig = loadConfig;
        }

        [Authorize]
        public async Task<IActionResult> Index()
        {
            return RedirectToAction("Index", "Tests");
        }
      
        [AllowAnonymous]
        public async Task<IActionResult> LogOut()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Register", "Identity/Account");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
