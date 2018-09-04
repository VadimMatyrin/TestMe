using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TestMe.Models;
using TestMe.Sevices.Interfaces;

namespace TestMe.Controllers
{
    [Authorize(Roles = "Admin, Moderator")]
    public class TestReportsController : Controller
    {
        private readonly ITestingPlatform _testingPlatform;
        private readonly UserManager<AppUser> _userManager;
        public TestReportsController(ITestingPlatform testingPlatform, UserManager<AppUser> userManager)
        {
            _testingPlatform = testingPlatform;
            _userManager = userManager;
        }
        public IActionResult Index(int? id)
        {
            if (id is null)
                return NotFound();
            var test = _testingPlatform.TestManager.GetAll().Where(t => t.Id == id).FirstOrDefault();
            if (test is null)
                return NotFound();

            return View(test);
        }
    }
}