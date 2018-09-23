using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using TestMe.Models;
using TestMe.Sevices.Interfaces;

namespace TestMe.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly ITestingPlatform _testingPlatform;
        private readonly UserManager<AppUser> _userManager;
        private readonly IOptions<LoadConfig> _loadConfig;
        public ProfileController(ITestingPlatform testingPlatform, UserManager<AppUser> userManager, IOptions<LoadConfig> loadConfig)
        {
            _testingPlatform = testingPlatform;
            _userManager = userManager;
            _loadConfig = loadConfig;
        }
        [HttpGet]
        [ActionName("Index")]
        public async Task<IActionResult> IndexGet(string id, string searchString)
        {
            ProfileModel profile;
            if (id is null)
            {
                profile = new ProfileModel
                {
                    AppUser = await _userManager.FindByNameAsync(User.Identity.Name)
                };
            }
            else
            {
                profile = new ProfileModel
                {
                    AppUser = await _userManager.FindByIdAsync(id)
                };
            }
            if (searchString is null)
                searchString = "";

            profile.UserTests = await _testingPlatform.TestManager
                .GetAll()
                .Where(t => t.AppUserId == profile.AppUser.Id && t.TestCode != null && t.TestName.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                .Take(_loadConfig.Value.TakeAmount)
                .ToListAsync();

            return View(profile);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GetUserProfileTestsAjax(string userId, int? skipAmount, string searchString)
        {
            if (userId is null || skipAmount is null)
                return BadRequest();

            if (searchString is null)
                searchString = "";

            var tests = await _testingPlatform.TestManager
                .GetAll()
                .Where(t => t.AppUserId == userId && t.TestCode != null && t.TestName.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                .Skip(skipAmount.Value)
                .Take(_loadConfig.Value.AjaxTakeAmount)
                .ToListAsync();

            var optimizedTests = tests.Select(t =>
            new
            {
                t.TestName,
                t.CreationDate,
                t.TestDuration,
                testRating = t.TestMarks.Count(tm => tm.EnjoyedTest) - t.TestMarks.Count(tm => !tm.EnjoyedTest)
            });
            return Json(optimizedTests);
        }
    }
}