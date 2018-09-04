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

        [HttpGet]
        [ActionName("Index")]
        public async Task<IActionResult> IndexGet(string searchString)
        {
            if (searchString is null)
                searchString = "";

            var tests = await _testingPlatform.TestManager.GetAll().Where(t => t.TestName.ToUpper().Contains(searchString.ToUpper())).Take(10).ToListAsync();
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
        public async Task<IActionResult> AddToAdmins(string id)
        {
            if (String.IsNullOrEmpty(id))
                return RedirectToAction(nameof(Users));

            var user = await _userManager.FindByIdAsync(id);
            if (user is null)
                return NotFound();

            if (!(await _userManager.IsInRoleAsync(user, "Admin")))
               await _userManager.AddToRoleAsync(user, "Admin");

            return RedirectToAction(nameof(Users));
        }
        public async Task<IActionResult> RemoveFromAdmins(string id)
        {
            if (String.IsNullOrEmpty(id))
                return RedirectToAction(nameof(Users));

            var user = await _userManager.FindByIdAsync(id);
            if (user is null)
                return NotFound();

            if(await _userManager.IsInRoleAsync(user, "Admin") && user.UserName != User.Identity.Name && user.UserName != "admin")
                await _userManager.RemoveFromRoleAsync(user, "Admin");

            return RedirectToAction(nameof(Users));
        }
    }
}