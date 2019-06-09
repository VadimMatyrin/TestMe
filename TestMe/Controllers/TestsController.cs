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
using TestMe.Exceptions;

namespace TestMe.Controllers
{
    [Authorize]
    public class TestsController : Controller
    {
        private readonly ITestingPlatform _testingPlatform;
        private readonly UserManager<AppUser> _userManager;
        private readonly IOptions<LoadConfig> _loadConfig;
        private string _userId;
        public class FilterModel
        {
            public string SearchString { get; set; }
            public int? TestRatingFrom { get; set; }
            public int? TestRatingTo { get; set; }
            public TimeSpan? TestDurationFrom { get; set; }
            public TimeSpan? TestDurationTo { get; set; }
            public void SwapRatingBounds()
            {
                if (TestRatingFrom > TestRatingTo)
                {
                    int tmp = TestRatingFrom.Value;
                    TestRatingFrom = TestRatingTo;
                    TestRatingTo = tmp;
                }
            }
            public void SwapDurationBounds()
            {
                if (TestDurationFrom > TestDurationTo)
                {
                    var tmp = TestDurationFrom;
                    TestDurationFrom = TestDurationTo;
                    TestDurationTo = tmp;
                }
            }
        }
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
                .Include(t => t.TestQuestions)
                .Include(t => t.TestResults)
                .Include(t => t.AppUser)
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

            var test = await _testingPlatform.TestManager.GetAll().Include(t => t.TestQuestions).FirstOrDefaultAsync(t => t.AppUserId == _userId && t.Id == id);
            if (test is null)
                return NotFound();

            if (!(test.TestCode is null))
                return NotFound();

            foreach (var testQuestion in test.TestQuestions)
            {
                var testAnswers = await _testingPlatform.TestAnswerManager.GetAll().Where(ta => ta.TestQuestionId == testQuestion.Id).ToListAsync();
                testQuestion.TestAnswers = testAnswers;
            }

            return View(test);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Test test)
        {
            if (test is null)
                throw new TestNotFoundException();

            test.AppUserId = _userId;
            if (!(test.TestQuestions is null))
            {
                foreach (var testQuestion in test.TestQuestions.Where(tq => !(tq is null)))
                {
                    testQuestion.AppUserId = _userId;
                    foreach (var testAnswer in test.TestAnswers.Where(ta => !(ta is null)))
                    {
                        testAnswer.AppUserId = _userId;
                    }
                }
            }
            await _testingPlatform.TestManager.UpdateAsync(test);
            return RedirectToAction(nameof(Index));

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
            if (_userId != _userManager.GetUserId(User))
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

            ViewBag.AjaxTakeAmount = _loadConfig.Value.AjaxTakeAmount;
            return View(topRatedTests);
        }
        [HttpGet]
        [ActionName("SearchTests")]
        public async Task<IActionResult> SearchTestsGet(FilterModel filterModel)
        {
            if (filterModel.SearchString is null)
                filterModel.SearchString = "";

            if (filterModel.TestRatingFrom is null || filterModel.TestRatingFrom < 0)
                filterModel.TestRatingFrom = 0;

            if (filterModel.TestRatingTo is null || filterModel.TestRatingTo < 0)
                filterModel.TestRatingTo = Int32.MaxValue;

            filterModel.SwapRatingBounds();

            if (filterModel.TestDurationFrom is null)
                filterModel.TestDurationFrom = new TimeSpan(0, 0, 0);

            if (filterModel.TestDurationTo is null)
                filterModel.TestDurationTo = new TimeSpan(23, 59, 0);

            filterModel.SwapDurationBounds();

            var tests = await _testingPlatform.TestManager
             .GetAll()
             .Where(t => t.TestCode != null &&
               t.TestName.Contains(filterModel.SearchString, StringComparison.OrdinalIgnoreCase) &&
               t.TestMarks.Count(tm => tm.EnjoyedTest) - t.TestMarks.Count(tm => !tm.EnjoyedTest) >= filterModel.TestRatingFrom &&
               t.TestMarks.Count(tm => tm.EnjoyedTest) - t.TestMarks.Count(tm => !tm.EnjoyedTest) <= filterModel.TestRatingTo &&
               t.TestDuration >= filterModel.TestDurationFrom &&
               t.TestDuration <= filterModel.TestDurationTo)
             .Take(_loadConfig.Value.TakeAmount)
             .ToListAsync();

            if (tests is null)
                return NotFound();

            ViewBag.AjaxTakeAmount = _loadConfig.Value.AjaxTakeAmount;
            return View(tests);
        }

        [HttpGet]
        public async Task<IActionResult> TestRecord(int? id)
        {
            if (id is null)
                return BadRequest();

            var test = await _testingPlatform.TestManager
                .GetAll()
                .FirstOrDefaultAsync(t => t.Id == id);

            var testResults = await _testingPlatform.TestResultManager
                .GetAll()
                .Where(tr => tr.TestId == test.Id)
                .ToListAsync();

            if (test is null)
                return BadRequest();

            if (testResults is null)
                return BadRequest();

            test.TestResults = testResults;

            return View(test);
        }
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GetTestsAjax(int? skipAmount, FilterModel filterModel)
        {
            if (skipAmount is null)
                return BadRequest();

            if (filterModel.SearchString is null)
                filterModel.SearchString = "";

            var tests = await _testingPlatform.TestManager
                .GetAll()
                .Where(t => t.TestName.Contains(filterModel.SearchString, StringComparison.OrdinalIgnoreCase))
                .Skip(skipAmount.Value)
                .Take(_loadConfig.Value.AjaxTakeAmount)
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
        public async Task<IActionResult> GetReportedTestsAjax(int? skipAmount, FilterModel filterModel)
        {
            if (skipAmount is null)
                return NotFound();

            if (filterModel.SearchString is null)
                filterModel.SearchString = "";

            var tests = await _testingPlatform.TestManager
                .GetAll()
                .Where(t => t.TestReports.Count >= _loadConfig.Value.MinReportAmount && t.TestName.Contains(filterModel.SearchString, StringComparison.OrdinalIgnoreCase))
                .Skip(skipAmount.Value)
                .Take(_loadConfig.Value.AjaxTakeAmount)
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
        public async Task<IActionResult> GetSharedTestsAjax(int? skipAmount, FilterModel filterModel)
        {
            if (skipAmount is null)
                return NotFound();

            if (filterModel.SearchString is null)
                filterModel.SearchString = "";

            if (filterModel.TestRatingFrom is null || filterModel.TestRatingFrom < 0)
                filterModel.TestRatingFrom = 0;

            if (filterModel.TestRatingTo is null || filterModel.TestRatingTo < 0)
                filterModel.TestRatingTo = Int32.MaxValue;

            filterModel.SwapRatingBounds();

            if (filterModel.TestDurationFrom is null)
                filterModel.TestDurationFrom = new TimeSpan(0, 0, 0);

            if (filterModel.TestDurationTo is null)
                filterModel.TestDurationTo = new TimeSpan(23, 59, 0);

            filterModel.SwapDurationBounds();

            var tests = await _testingPlatform.TestManager
             .GetAll()
             .Where(t => t.TestCode != null &&
               t.TestName.Contains(filterModel.SearchString, StringComparison.OrdinalIgnoreCase) &&
               t.TestMarks.Count(tm => tm.EnjoyedTest) - t.TestMarks.Count(tm => !tm.EnjoyedTest) >= filterModel.TestRatingFrom &&
               t.TestMarks.Count(tm => tm.EnjoyedTest) - t.TestMarks.Count(tm => !tm.EnjoyedTest) <= filterModel.TestRatingTo &&
               t.TestDuration >= filterModel.TestDurationFrom &&
               t.TestDuration <= filterModel.TestDurationTo)
             .Skip(skipAmount.Value)
             .Take(_loadConfig.Value.AjaxTakeAmount)
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
        public async Task<IActionResult> GetTopTestsAjax(int? skipAmount, FilterModel filterModel)
        {
            if (skipAmount is null)
                return NotFound();

            if (filterModel.SearchString is null)
                filterModel.SearchString = "";

            var topRatedTests = await _testingPlatform.TestManager
                .GetAll()
                .Where(t =>
                t.TestCode != null && t.TestName.Contains(filterModel.SearchString, StringComparison.OrdinalIgnoreCase)
                && t.TestMarks.Count(tm => tm.EnjoyedTest) - t.TestMarks.Count(tm => !tm.EnjoyedTest) >= _loadConfig.Value.MinTopRatedRate)
                .OrderByDescending(t => t.TestMarks.Count(tm => tm.EnjoyedTest) - t.TestMarks.Count(tm => !tm.EnjoyedTest))
                .Skip(skipAmount.Value)
                .Take(_loadConfig.Value.AjaxTakeAmount)
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
