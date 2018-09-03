using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TestMe.Models;
using TestMe.Sevices.Interfaces;

namespace TestMe.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ITestingPlatform _testingPlatform;
        private readonly UserManager<AppUser> _userManager;
        public AdminController(ITestingPlatform testingPlatform, UserManager<AppUser> userManager)
        {
            _testingPlatform = testingPlatform;
            _userManager = userManager;
        }
        public async Task<IActionResult> Index()
        {
            var tests = await _testingPlatform.TestManager.GetAll().Take(10).ToListAsync();
            if (tests is null)
                return NotFound();

            return View(tests);
        }

        public async Task <IActionResult> Users()
        {
            var appUsers = await _userManager.Users.AsNoTracking().Take(10).ToListAsync();//_testingPlatform.TestManager.GetAll().Take(10);
            if (appUsers is null)
                return NotFound();

            return View(appUsers);
        }
    }
}