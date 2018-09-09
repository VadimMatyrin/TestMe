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

            profile.TestMarks = _testingPlatform.TestMarkManager.GetAll().Where(tm => tm.AppUserId == profile.AppUser.Id).ToList();
            profile.UserTests = _testingPlatform.TestManager.GetAll().Where(t => t.AppUserId == profile.AppUser.Id && t.TestCode != null && t.TestName.Contains(searchString)).Take(1).ToList();
            profile.TestResults = _testingPlatform.TestResultManager.GetAll().Where(tm => tm.AppUserId == profile.AppUser.Id).ToList();
            return View(profile);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult GetUserProfileTestsAjax(string userId, int? skipAmount, int? amount)
        {
            if (skipAmount is null || amount is null || userId is null)
                return NotFound();

            var searchString = "";
            if (HttpContext.Request.Query.Count != 0 && HttpContext.Request.Query["searchString"] != "")
            {
                searchString = HttpContext.Request.Query["searchString"];
            }

            var tests = _testingPlatform.TestManager
                .GetAll()
                .Where(t => t.AppUserId == userId && t.TestCode != null && t.TestName.Contains(searchString))
                .Skip(skipAmount.Value)
                .Take(amount.Value)
                .ToList();
            var optimizedTests = tests.Select(t =>
            new
            {
                testName = t.TestName,
                creationDate = t.CreationDate,
                duration = t.TestDuration,
                testRating = t.TestMarks.Count(tm => tm.EnjoyedTest) - t.TestMarks.Count(tm => !tm.EnjoyedTest)
            }
            );
            return Json(optimizedTests);
        }
    }
}