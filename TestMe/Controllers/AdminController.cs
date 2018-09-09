using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        public AdminController(ITestingPlatform testingPlatform, UserManager<AppUser> userManager)
        {
            _testingPlatform = testingPlatform;
            _userManager = userManager;
        }

        [HttpGet]
        [ActionName("Index")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> IndexGet(string searchString)
        {
            List<Test> tests;
            if (searchString is null)
                tests = await _testingPlatform.TestManager
                    .GetAll()
                    .Take(1)
                    .ToListAsync();
            else
               tests = await _testingPlatform.TestManager
                    .GetAll()
                    .Where(t => t.TestName.Contains(searchString.ToUpper()))
                    .Take(1)
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
            List<AppUser> appUsers;

            if (searchString is null)
                appUsers = await _userManager.Users.AsNoTracking()
                    .Take(1)
                    .ToListAsync();
            else
                appUsers = await _userManager.Users.AsNoTracking()
                    .Where(u => u.NormalizedUserName.Contains(searchString.ToUpper()))
                    .Take(1)
                    .ToListAsync();

            if (appUsers is null)
                return NotFound();

            return View(appUsers);
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
            List<Test> tests;
            if (searchString is null)
                tests = await _testingPlatform.TestManager
                    .GetAll()
                    .Where(t => t.TestReports.Count >= 1)
                    .OrderByDescending(t => t.TestReports.Count)
                    .Take(1)
                    .ToListAsync();
            else
                tests = await _testingPlatform.TestManager
                    .GetAll()
                    .Where(t => t.TestReports.Count >= 1 && t.TestName.Contains(searchString))
                    .OrderByDescending(t => t.TestReports.Count)
                    .Take(1)
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
        public async Task<IActionResult> GetUsersAjax(int? skipAmount, int? amount)
        {
            if (skipAmount is null || amount is null)
                return BadRequest();

            var searchString = "";
            if (HttpContext.Request.Query.Count!=0 && HttpContext.Request.Query["searchString"] != "")
            {
                searchString = HttpContext.Request.Query["searchString"];
            }
            var users = await _userManager.Users.AsNoTracking()
                .Where(u => u.UserName.Contains(searchString))
                .Skip(skipAmount.Value)
                .Take(amount.Value)
                .ToListAsync();
            if (users is null)
                return NotFound();

            var usersOptimized = await Task.WhenAll(users.Select(async u =>
            new
            {
                id = u.Id,
                userId = u.Id,
                userName = u.UserName,
                name = u.Name,
                surname = u.Surname,
                email = u.Email,
                phoneNumber = u.PhoneNumber,
                role = (await _userManager.IsInRoleAsync(u, "Admin")) ? "Admin" : (await _userManager.IsInRoleAsync(u, "Moderator") ? "Moderator" : null),
                currentUserUsername = User.Identity.Name
            }));

            return Json(usersOptimized);
        }
    }
}