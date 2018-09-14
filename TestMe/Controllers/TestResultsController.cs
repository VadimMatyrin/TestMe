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
    [Authorize]
    public class TestResultsController : Controller
    {
        private readonly ITestingPlatform _testingPlatform;
        private readonly UserManager<AppUser> _userManager;
        public TestResultsController(ITestingPlatform testingPlatform, UserManager<AppUser> userManager)
        {
            _testingPlatform = testingPlatform;
            _userManager = userManager;
        }
        public async Task<IActionResult> Index(int? id)
        {
            if (id is null)
                return NotFound();

            var test = await _testingPlatform.TestManager
                .FindAsync(t => t.Id == id);
            if (test is null)
                return NotFound();

            var testResults = await _testingPlatform.TestResultManager
                .GetAll()
                .Where(tr => tr.TestId == id)
                .ToListAsync();

            test.TestResults = testResults;

            return View(test);
        }
        public async Task<IActionResult> UserResults()
        {
            var userId = _userManager.GetUserId(User);
            var tests = await _testingPlatform.TestManager
                .GetAll()
                .Where(t => t.TestResults.Any(tr => tr.AppUserId == userId))
                .ToListAsync();

            foreach(var test in tests)
            {
                test.TestResults = test.TestResults.Where(tr => tr.AppUserId == userId).ToList();
                test.TestMarks = test.TestMarks.Where(tr => tr.AppUserId == userId).ToList();
            }
            return View(tests);
        }
    }
}