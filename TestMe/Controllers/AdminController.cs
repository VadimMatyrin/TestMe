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

            if (tests is null)
                return NotFound();

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

            if (appUsers is null)
                return NotFound();

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

            if (!(await _userManager.IsInRoleAsync(user, "Admin")))
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

            var user = await _userManager.FindByIdAsync(id);
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

            if (tests is null)
                return NotFound();

            return View(tests);
        }
        [Authorize(Roles = "Admin, Moderator")]
        public async Task<IActionResult> DeleteReports(int? id)
        {
            if (id is null)
                return NotFound();

            var testReports = await _testingPlatform.TestReportManager
                .GetAll()
                .Where(tr => tr.TestId == id)
                .ToListAsync();
            if (testReports is null)
                return NotFound();

            await _testingPlatform.TestReportManager.DeleteRangeAsync(testReports);

            return RedirectToAction("ReportedTests");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GetUsersAjax(int? skipAmount, int? amount, string searchString)
        {
            if (skipAmount is null || amount is null)
                return BadRequest();

            if (searchString is null)
                searchString = "";

            var users = await _userManager.Users.AsNoTracking()
                .Where(u => u.UserName.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                .Skip(skipAmount.Value)
                .Take(amount.Value)
                .ToListAsync();

            if (users is null)
                return NotFound();

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
                role = (await _userManager.IsInRoleAsync(u, "Admin")) ? "Admin" : (await _userManager.IsInRoleAsync(u, "Moderator") ? "Moderator" : null),
                currentUserUsername = User.Identity.Name
            }));

            return Json(usersOptimized);
        }
    }
}