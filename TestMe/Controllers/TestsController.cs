using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TestMe.Data;
using TestMe.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Filters;
using TestMe.Sevices.Interfaces;
using System.Text;
using Microsoft.Extensions.Options;

namespace TestMe.Controllers
{
    [Authorize]
    public class TestsController : Controller
    {
        private readonly ITestingPlatform _testingPlatform;
        private readonly UserManager<AppUser> _userManager;
        private readonly IOptions<LoadConfig> _loadConfig;
        private string _userId;
        public TestsController(ITestingPlatform testingPlatform, UserManager<AppUser> userManager, IOptions<LoadConfig> loadConfig)
        {
            _testingPlatform = testingPlatform;
            _userManager = userManager;
            _loadConfig = loadConfig;
        }
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (User.IsInRole("Admin"))
            {
                if (!(context.RouteData.Values["id"] is null))
                    if (Int32.TryParse(context.RouteData.Values["id"].ToString(), out int testId))
                        _userId = _testingPlatform.TestManager.GetAll().AsNoTracking().FirstOrDefault(t => t.Id == testId)?.AppUserId;
            }
            _userId = _userId ?? _userManager.GetUserId(User);
        }
        public async Task<IActionResult> Index()
        {
            var tests = await _testingPlatform.TestManager
                .GetAll()
                .Where(t => t.AppUserId == _userId)
                .ToListAsync();

            return View(tests);
        }
        public async Task<IActionResult> StopSharing(int? id)
        {
            if (id is null)
                return NotFound();

            var test = await _testingPlatform.TestManager.FindAsync(t => t.AppUserId == _userId && t.Id == id);
            if (test is null)
                return NotFound();

            test.TestCode = null;
            await _testingPlatform.TestManager.UpdateAsync(test);
            if (_userId != _userManager.GetUserId(User))
                return RedirectToAction("Index", "Admin");

            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> CreateCode(int? id)
        {
            if (id is null)
                return NotFound();

            var test = await _testingPlatform.TestManager.FindAsync(t => t.AppUserId == _userId && t.Id == id);
                
            if (test is null)
                return NotFound();

            if (await HasValidationErrorsAsync(id))
                return RedirectToAction(nameof(ValidateTest), new { id });

            if (test.TestCode is null)
            {
                var generatedCode = _testingPlatform.RandomStringGenerator.RandomString(8);

                test.TestCode = generatedCode;
                await _testingPlatform.TestManager.UpdateAsync(test);

                var testResult = _testingPlatform.TestResultManager.GetAll().Where(tr => tr.TestId == test.Id);
                await _testingPlatform.TestResultManager.DeleteRangeAsync(testResult);

                var testMarks = _testingPlatform.TestMarkManager.GetAll().Where(tm => tm.TestId == test.Id);
                await _testingPlatform.TestMarkManager.DeleteRangeAsync(testMarks);

            }
            return View("CreateCode", test);
        }
        public async Task<IActionResult> Details(int? id)
        {
            if (id is null)
                return NotFound();

            var test = await _testingPlatform.TestManager.FindAsync(t => t.Id == id);

            if (test is null)
                return NotFound();

            return View(test);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TestName,CreationDate, TestDuration")]Test test)
        {
            if (_testingPlatform.TestManager.GetAll().Where(t => t.AppUserId == _userId).Any(t => t.TestName == test.TestName))
            {
                ModelState.AddModelError("TestName", "You already have test with the same name!");
            }
            if (ModelState.IsValid)
            {
                test.AppUserId = _userId;
                await _testingPlatform.TestManager.AddAsync(test);
                return RedirectToAction(nameof(Index));
            }
            return View(test);
        }
        public async Task<IActionResult> Edit(int? id)
        {
            if (id is null)
            {
                return NotFound();
            }

            var test = await _testingPlatform.TestManager.FindAsync(t => t.AppUserId == _userId && t.Id == id);
            if (test is null)
                return NotFound();

            if (!(test.TestCode is null))
                return NotFound();

            return View(test);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id, TestName,CreationDate, TestDuration")] Test test)
        {
            if (id != test.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                test.AppUserId = _userId;
                await _testingPlatform.TestManager.UpdateAsync(test);
                return RedirectToAction(nameof(Index));
            }
            return View(test);
        }
        public async Task<IActionResult> Delete(int? id)
        {
            if (id is null)
                return NotFound();

            var test = await _testingPlatform.TestManager.FindAsync(t => t.Id == id && t.AppUserId == _userId);
            if (test is null)
                return NotFound();

            return View(test);
        }
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var test = await _testingPlatform.TestManager.FindAsync(t => t.AppUserId == _userId && t.Id == id);
            if (test is null)
                return NotFound();

            foreach (var testAnswer in test.TestAnswers.Where(ta => !(ta.ImageName is null)))
                _testingPlatform.AnswerImageManager.DeleteAnswerImage(testAnswer.ImageName);

            await _testingPlatform.TestManager.DeleteAsync(test);
            if(_userId != _userManager.GetUserId(User))
            {
                if (User.IsInRole("Admin"))
                    return RedirectToAction("Index", "Admin");
                return RedirectToAction("ReportedTests", "Admin");
            }
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> ValidateTest(int? id)
        {
            if (id is null)
                return NotFound();

            var test = await _testingPlatform.TestManager
                .FindAsync(t => t.AppUserId == _userId && t.Id == id);

            if (test is null)
                return NotFound();

            var errorModelTest = new Test();

            if (test.TestQuestions.Count == 0)
            {
                errorModelTest.TestQuestions = new List<TestQuestion>();
                return View(errorModelTest);
            }

            if (test.TestQuestions.Any(tq => tq.TestAnswers.Count == 0))
            {
                errorModelTest.TestQuestions = new List<TestQuestion>();
                errorModelTest.TestQuestions = errorModelTest.TestQuestions
                    .Concat(test.TestQuestions.Where(tq => tq.TestAnswers.Count == 0))
                    .ToList();
            }

            if (test.TestQuestions.Any(tq => tq.TestAnswers.All(ta => !ta.IsCorrect)))
            {
                errorModelTest.TestQuestions = new List<TestQuestion>();
                errorModelTest.TestQuestions = errorModelTest.TestQuestions
                    .Concat(test.TestQuestions.Where(tq => tq.TestAnswers.All(ta => !ta.IsCorrect)))
                    .ToList();
            }

            return View(errorModelTest);
        }

        [HttpGet]
        [ActionName("TopRated")]
        public async Task<IActionResult> TopRatedGet(string searchString)
        {
            if (searchString is null)
                searchString = "";

            var topRatedTests = await _testingPlatform.TestManager
                .GetAll()
                .Where(t => 
                    !(t.TestCode == null) &&
                    t.TestName.Contains(searchString, StringComparison.OrdinalIgnoreCase) && 
                    t.TestMarks.Count(tm => tm.EnjoyedTest) - t.TestMarks.Count(tm => !tm.EnjoyedTest) >= _loadConfig.Value.MinTopRatedRate)
                .Take(_loadConfig.Value.TakeAmount)
                .OrderByDescending(t => t.TestMarks.Count(tm => tm.EnjoyedTest) - t.TestMarks.Count(tm => !tm.EnjoyedTest))
                .ToListAsync();

            if (topRatedTests is null)
                return NotFound();

            return View(topRatedTests);
        }
        [HttpGet]
        [ActionName("SearchTests")]
        public async Task<IActionResult> SearchTestsGet(string searchString, int? testRatingFrom, int? testRatingTo)
        {
            if (searchString is null)
                searchString = "";

            if (testRatingFrom is null)
                testRatingFrom = 0;

            if (testRatingTo is null)
                testRatingTo = Int32.MaxValue;

            if (testRatingTo < testRatingFrom)
            {
                int tmp = testRatingTo.Value;
                testRatingTo = testRatingFrom;
                testRatingFrom = tmp;
            }

            var tests = await _testingPlatform.TestManager
              .GetAll()
              .Where(t => t.TestCode != null &&
                t.TestName.Contains(searchString, StringComparison.OrdinalIgnoreCase) &&
                t.TestMarks.Count(tm => tm.EnjoyedTest) - t.TestMarks.Count(tm => !tm.EnjoyedTest) >= testRatingFrom &&
                t.TestMarks.Count(tm => tm.EnjoyedTest) - t.TestMarks.Count(tm => !tm.EnjoyedTest) <= testRatingTo)
              .Take(_loadConfig.Value.TakeAmount)
              .ToListAsync();
            if (tests is null)
                return NotFound();

            return View(tests);
        }
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GetTestsAjax(int? skipAmount, int? amount, string searchString)
        {
            if (skipAmount is null || amount is null)
                return BadRequest();

            if (searchString is null)
                searchString = "";

            var tests = await _testingPlatform.TestManager
                .GetAll()
                .Where(t => t.TestName.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                .Skip(skipAmount.Value)
                .Take(amount.Value)
                .ToListAsync();

            var optimizedTests = tests.Select(t =>
            new
            {
                t.Id,
                t.TestName,
                t.CreationDate,
                t.TestCode,
                t.TestDuration,
                testRating = t.TestMarks.Count(tm => tm.EnjoyedTest) - t.TestMarks.Count(tm => !tm.EnjoyedTest),
                userId = t.AppUser.Id,
                t.AppUser.UserName
            });

            return Json(optimizedTests);
        }
        [HttpPost]
        [Authorize(Roles = "Admin, Moderator")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GetReportedTestsAjax(int? skipAmount, int? amount, string searchString)
        {
            if (skipAmount is null || amount is null)
                return NotFound();

            if (searchString is null)
                searchString = "";

            var tests = await _testingPlatform.TestManager
                .GetAll()
                .Where(t => t.TestReports.Count >= _loadConfig.Value.MinReportAmount && t.TestName.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                .Skip(skipAmount.Value)
                .Take(amount.Value)
                .ToListAsync();

            var optimizedTests = tests.Select(t =>
            new
            {
                t.Id,
                t.TestName,
                t.AppUser.UserName,
                t.AppUserId,
                t.TestCode,
                reportAmount = t.TestReports.Count,
                testRating = t.TestMarks.Count(tm => tm.EnjoyedTest) - t.TestMarks.Count(tm => !tm.EnjoyedTest),
            });

            return Json(optimizedTests);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GetSharedTestsAjax(int? skipAmount, int? amount, string searchString, int? testRatingFrom, int? testRatingTo)
        {
            if (skipAmount is null || amount is null)
                return NotFound();

            if (searchString is null)
                searchString = "";

            if (testRatingFrom is null)
                testRatingFrom = 0;

            if (testRatingTo is null)
                testRatingTo = Int32.MaxValue;

            if (testRatingTo < testRatingFrom)
            {
                int tmp = testRatingTo.Value;
                testRatingTo = testRatingFrom;
                testRatingFrom = tmp;
            }

            var tests = await _testingPlatform.TestManager
             .GetAll()
             .Where(t => t.TestCode != null &&
               t.TestName.Contains(searchString, StringComparison.OrdinalIgnoreCase) &&
               t.TestMarks.Count(tm => tm.EnjoyedTest) - t.TestMarks.Count(tm => !tm.EnjoyedTest) >= testRatingFrom &&
               t.TestMarks.Count(tm => tm.EnjoyedTest) - t.TestMarks.Count(tm => !tm.EnjoyedTest) <= testRatingTo)
             .Skip(skipAmount.Value)
             .Take(_loadConfig.Value.TakeAmount)
             .ToListAsync();

            var optimizedTests = tests.Select(t =>
            new
            {
                t.Id,
                t.TestName,
                t.CreationDate,
                t.TestCode,
                t.TestDuration,
                testRating = t.TestMarks.Count(tm => tm.EnjoyedTest) - t.TestMarks.Count(tm => !tm.EnjoyedTest),
                t.AppUser.UserName
            }); 

            return Json(optimizedTests);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GetTopTestsAjax(int? skipAmount, int? amount, string searchString)
        {
            if (skipAmount is null || amount is null)
                return NotFound();

            if (searchString is null)
                searchString = "";

            var topRatedTests = await _testingPlatform.TestManager
                .GetAll()
                .Where(t =>
                t.TestCode != null && t.TestName.Contains(searchString, StringComparison.OrdinalIgnoreCase) 
                && t.TestMarks.Count(tm => tm.EnjoyedTest) - t.TestMarks.Count(tm => !tm.EnjoyedTest) >= _loadConfig.Value.MinTopRatedRate)
                .OrderByDescending(t => t.TestMarks.Count(tm => tm.EnjoyedTest) - t.TestMarks.Count(tm => !tm.EnjoyedTest))
                .Skip(skipAmount.Value)
                .Take(amount.Value)
                .ToListAsync();
            
            var optimizedTests = topRatedTests.Select(t =>
            new
            {
                t.Id,
                t.TestName,
                t.CreationDate,
                t.TestDuration,
                t.TestCode,
                testRating = t.TestMarks.Count(tm => tm.EnjoyedTest) - t.TestMarks.Count(tm => !tm.EnjoyedTest),
            });

            return Json(optimizedTests);
        }
        private async Task<bool> HasValidationErrorsAsync(int? id)
        {
            if (id is null)
                return true;

            var test = await _testingPlatform.TestManager
                .FindAsync(t => t.AppUserId == _userId && t.Id == id);

            if (test is null)
                return true;

            if (test.TestQuestions.Count == 0)
                return true;

            if (test.TestQuestions.Any(tq => tq.TestAnswers.Count == 0))
                return true;

            if (test.TestQuestions.Any(tq => tq.TestAnswers.All(ta => !ta.IsCorrect)))
                return true;
            
            return false;
        }
    }
}
