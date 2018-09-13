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

            var test = await _testingPlatform.TestManager.FindAsync(t => t.Id == id);
            if (test is null)
                return NotFound();
            var testResults = _testingPlatform.TestResultManager.GetAll().Where(tm => tm.TestId == id).ToList();
            if (testResults is null)
                return NotFound();
            test.TestResults = testResults;
            var questionAmount = _testingPlatform.TestQuestionManager.GetAll().Count(tq => tq.TestId == id);
            ViewBag.questionAmount = questionAmount;
            return View(test);
        }
        public async Task<IActionResult> UserResults()
        {
            var userId = _userManager.GetUserId(User);
            var tests = _testingPlatform.TestManager.GetAll().Where(t => t.TestResults.Any(tr => tr.AppUserId == userId)).ToList();
            foreach(var test in tests)
            {
                test.TestResults = test.TestResults.Where(tr => tr.AppUserId == userId).ToList();
                test.TestMarks = test.TestMarks.Where(tr => tr.AppUserId == userId).ToList();
            }
            return View(tests);
        }
    }
}