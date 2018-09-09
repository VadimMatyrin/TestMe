using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TestMe.Models;
using TestMe.Sevices.Interfaces;

namespace TestMe.Controllers
{
    public class ProfileController : Controller
    {
        private readonly ITestingPlatform _testingPlatform;
        private readonly UserManager<AppUser> _userManager;
        public ProfileController(ITestingPlatform testingPlatform, UserManager<AppUser> userManager)
        {
            _testingPlatform = testingPlatform;
            _userManager = userManager;
        }
        public async Task<IActionResult> Index(string id)
        {
            if (id is null)
                return NotFound();
            var profile = new ProfileModel
            {
                AppUser = await _userManager.FindByIdAsync(id)
            };
            profile.TestMarks = _testingPlatform.TestMarkManager.GetAll().Where(tm => tm.AppUserId == profile.AppUser.Id).ToList();
            profile.UserTests = _testingPlatform.TestManager.GetAll().Where(tm => tm.AppUserId == profile.AppUser.Id).ToList();
            profile.TestResults = _testingPlatform.TestResultManager.GetAll().Where(tm => tm.AppUserId == profile.AppUser.Id).ToList();
            return View(profile);
        }
    }
}