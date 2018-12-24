using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using TestMe.Models;
using TestMe.Sevices.Interfaces;
using TestMe.ViewModels;

namespace TestMe.Controllers
{
    [Authorize(Roles = "Admin, Moderator")]
    public class AdminController : Controller
    {
        private readonly ITestingPlatform _testingPlatform;
        private readonly UserManager<AppUser> _userManager;
        private readonly IOptions<LoadConfig> _loadConfig;
        public AdminController(ITestingPlatform testingPlatform, UserManager<AppUser> userManager, IOptions<LoadConfig> loadConfig)
        {
            _testingPlatform = testingPlatform;
            _userManager = userManager;
            _loadConfig = loadConfig;
        }

        [HttpGet]
        [ActionName("Index")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> IndexGet(string searchString)
        {
            if (searchString is null)
                searchString = "";

            var tests = await _testingPlatform.TestManager
                 .GetAll()
                 .Where(t => t.TestName.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                 .Take(_loadConfig.Value.TakeAmount)
                 .ToListAsync();

            ViewBag.AjaxTakeAmount = _loadConfig.Value.AjaxTakeAmount;
            return View(tests);
        }
        [HttpGet]
        [ActionName("Users")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UsersGet(string searchString)
        {
            if (searchString is null)
                searchString = "";

            var appUsers = await _userManager.Users.AsNoTracking()
                    .Where(u => u.NormalizedUserName.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                    .Take(_loadConfig.Value.TakeAmount)
                    .ToListAsync();

            ViewBag.AjaxTakeAmount = _loadConfig.Value.AjaxTakeAmount;
            return View(appUsers);
        }
        [Authorize(Roles ="Admin")]
        public async Task<IActionResult> BanUser(string id)
        {
            await ChangeUserBanStatusAsync(id, true);
            return RedirectToAction("Users");
        }
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UnBanUser(string id)
        {
            await ChangeUserBanStatusAsync(id, false);
            return RedirectToAction("Users");
        }
        private async Task<bool> ChangeUserBanStatusAsync(string id, bool isBanned)
        {
            if (String.IsNullOrEmpty(id))
                return false;

            var user = await _userManager.FindByIdAsync(id);
            if (user is null)
                return false;

            if (!(await _userManager.IsInRoleAsync(user, "Admin")) && User.Identity.Name != user.UserName)
            {
                if (user.IsBanned != isBanned)
                {
                    user.IsBanned = isBanned;
                    await _userManager.UpdateAsync(user);
                }
            }

            return true;
        }
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UsersRecord()
        {
            var users = await _userManager.Users.Take(20).ToListAsync();
            var viewModels = new List<UsersRecordViewModel>();
            foreach (var user in users)
            {
                var userTests = await _testingPlatform.TestManager.GetAll().Where(t => t.AppUserId == user.Id).ToListAsync();
                var userResults = await _testingPlatform.TestResultManager.GetAll().Where(tr => tr.AppUserId == user.Id).ToListAsync();
                double avgMark = 0;
                if (userTests.Count != 0)
                {
                    var sharedTests = userTests.Where(t => !(t.TestCode is null));
                    if(sharedTests.Count() != 0)
                        avgMark = sharedTests.Average(t => t.TestMarks.Count(tm => tm.EnjoyedTest) - t.TestMarks.Count(tm => !tm.EnjoyedTest));
                }
                    

                var viewModel = new UsersRecordViewModel
                {
                    User = user,
                    Tests = userTests,
                    TestResults = userResults,
                    AvgTestMark = avgMark
                };

                viewModels.Add(viewModel);
            }
            return View(viewModels);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddToAdmins(string id)
        {
            if (String.IsNullOrEmpty(id))
                return RedirectToAction("Users");

            var user = await _userManager.FindByIdAsync(id);
            if (user is null)
                return NotFound();

            if (!(await _userManager.IsInRoleAsync(user, "Admin")))
                await _userManager.AddToRoleAsync(user, "Admin");

            return RedirectToAction("Users");
        }
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RemoveFromAdmins(string id)
        {
            if (String.IsNullOrEmpty(id))
                return RedirectToAction("Users");

            var user = await _userManager.FindByIdAsync(id);
            if (user is null)
                return NotFound();

            if (await _userManager.IsInRoleAsync(user, "Admin") && user.UserName != User.Identity.Name && user.UserName != "admin")
                await _userManager.RemoveFromRoleAsync(user, "Admin");

            return RedirectToAction("Users");
        }
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddToModerators(string id)
        {
            if (String.IsNullOrEmpty(id))
                return RedirectToAction("Users");

            var user = await _userManager.FindByIdAsync(id);
            if (user is null)
                return NotFound();

            if (!(await _userManager.IsInRoleAsync(user, "Moderator")))
                await _userManager.AddToRoleAsync(user, "Moderator");

            return RedirectToAction("Users");
        }
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RemoveFromModerators(string id)
        {
            if (String.IsNullOrEmpty(id))
                return RedirectToAction("Users");

            var   = await _userManager.FindByIdAsync(id);
            if (user is null)
                return NotFound();

            if (await _userManager.IsInRoleAsync(user, "Moderator") && user.UserName != User.Identity.Name)
                await _userManager.RemoveFromRoleAsync(user, "Moderator");

            return RedirectToAction("Users");
        }
        [HttpGet]
        [ActionName("ReportedTests")]
        [Authorize(Roles = "Admin, Moderator")]
        public async Task<IActionResult> ReportedTestsGet(string searchString)
        {
            if (searchString is null)
                searchString = "";

            var tests = await _testingPlatform.TestManager
                  .GetAll()
                  .Where(t => t.TestReports.Count >= _loadConfig.Value.MinReportAmount && t.TestName.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                  .OrderByDescending(t => t.TestReports.Count)
                  .Take(_loadConfig.Value.TakeAmount)
                  .ToListAsync();

            ViewBag.AjaxTakeAmount = _loadConfig.Value.AjaxTakeAmount;
            return View(tests);
        }
        [Authorize(Roles = "Admin, Moderator")]
        public async Task<IActionResult> DeleteReports(int? id)
        {
            if (id is null)
                return NotFound();

            var testReports = _testingPlatform.TestReportManager
                .GetAll()
                .Where(tr => tr.TestId == id);

            await _testingPlatform.TestReportManager.DeleteRangeAsync(testReports);

            return RedirectToAction("ReportedTests");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GetUsersAjax(int? skipAmount, string searchString)
        {
            if (skipAmount is null)
                return BadRequest();

            if (searchString is null)
                searchString = "";

            var users = await _userManager.Users.AsNoTracking()
                .Where(u => u.UserName.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                .Skip(skipAmount.Value)
                .Take(_loadConfig.Value.AjaxTakeAmount)
                .ToListAsync();

            var usersOptimized = await Task.WhenAll(users.Select(async u =>
            new
            {
                u.Id,
                u.UserName,
                u.Name,
                u.Surname,
                u.Email,
                u.PhoneNumber,
                u.IsBanned,
                role = await _userManager.GetRolesAsync(u), //(await _userManager.IsInRoleAsync(u, "Admin")) ? "Admin" : (await _userManager.IsInRoleAsync(u, "Moderator") ? "Moderator" : null),
                currentUserUsername = User.Identity.Name
            }));

            return Json(usersOptimized);
        }
    }
}