using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using TestMe.Models;
using TestMe.Sevices.Interfaces;

namespace TestMe.Controllers
{
    [Authorize]
    public class TestReportsController : Controller
    {
        private readonly ITestingPlatform _testingPlatform;
        private readonly UserManager<AppUser> _userManager;
        private string _userId;
        public TestReportsController(ITestingPlatform testingPlatform, UserManager<AppUser> userManager)
        {
            _testingPlatform = testingPlatform;
            _userManager = userManager;
        }
        public override void OnActionExecuting(ActionExecutingContext context)
        { 
            _userId = _userManager.GetUserId(User);
        }
        [Authorize(Roles = "Admin, Moderator")]
        public async Task<IActionResult> Index(int? id)
        {
            if (id is null)
                return NotFound();

            var testReports = await _testingPlatform.TestReportManager
                .GetAll()
                .Where(tr => tr.Test.Id == id)
                .ToListAsync();

            return View(testReports);
        }
        public async Task<IActionResult> Create(int? id)
        {
            if (id is null)
                return NotFound();

            var test = await _testingPlatform.TestManager
                .FindAsync(t => t.TestCode != null && t.Id == id);
            if (test is null)
                return NotFound();

            var reports = await _testingPlatform.TestReportManager
                .GetAll()
                .Where(tr => tr.AppUserId == _userId && tr.TestId == test.Id)
                .ToListAsync();

            if (reports.Count() != 0)
                return RedirectToAction(nameof(Index), "TestEngine", new { code = test.TestCode });

            return View(new TestReport { Test = test, TestId = test.Id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Message, TestId")] TestReport testReport)
        {
            if (ModelState.IsValid)
            {
                testReport.AppUserId = _userId;
                var test = await _testingPlatform.TestManager.FindAsync(t => t.Id == testReport.TestId);
                if (test is null)
                    return View(testReport);

                await _testingPlatform.TestReportManager.AddAsync(testReport);
                return RedirectToAction(nameof(Index), "TestEngine", new { code = test.TestCode});
            }
            return View(testReport);
        }
    }
}