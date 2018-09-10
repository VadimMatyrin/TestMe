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
        private readonly IOptions<MyConfig> _config;
        private string _userId;
        public TestsController(ITestingPlatform testingPlatform, UserManager<AppUser> userManager, IOptions<MyConfig> config)
        {
            _testingPlatform = testingPlatform;
            _userManager = userManager;
            _config = config;
        }
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (User.IsInRole("Admin"))
            {
                if (!(context.RouteData.Values["id"] is null))
                    if (Int32.TryParse(context.RouteData.Values["id"].ToString(), out int testId))
                        _userId = _testingPlatform.TestManager.GetAll().AsNoTracking().Where(t => t.Id == testId).ToList().FirstOrDefault()?.AppUserId;
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
            if (id == null)
            {
                return NotFound();
            }

            var test = await _testingPlatform.TestManager.GetTestAsync(_userId, id);
            if (test == null)
            {
                return NotFound();
            }
            try
            {
                test.TestCode = null;
                await _testingPlatform.TestManager.UpdateAsync(test);
            }
            catch (DbUpdateException)
            {
                if (_testingPlatform.TestManager.GetTestAsync(_userId, id) is null)
                {
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    throw;
                }
            }
            if (_userId != _userManager.GetUserId(User))
            {
                return RedirectToAction("Index", "Admin");
            }
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> CreateCode(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var test = await _testingPlatform.TestManager.GetTestAsync(_userId, id);
                
            if (test == null)
            {
                return NotFound();
            }

            if (HasValidationErrors(id))
                return RedirectToAction(nameof(ValidateTest), new { id });

            if (test.TestCode is null)
            {
                var generatedCode = _testingPlatform.RandomStringGenerator.RandomString(8);
                try
                {
                    test.TestCode = generatedCode;
                    await _testingPlatform.TestManager.UpdateAsync(test);
                }
                catch (DbUpdateException)
                {
                    if (_testingPlatform.TestManager.GetTestAsync(_userId, id) is null)
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                var testResult = _testingPlatform.TestResultManager.GetAll().Where(tr => tr.TestId == test.Id);
                await _testingPlatform.TestResultManager.DeleteRangeAsync(testResult);
                var testMarks = _testingPlatform.TestMarkManager.GetAll().Where(tm => tm.TestId == test.Id);
                await _testingPlatform.TestMarkManager.DeleteRangeAsync(testMarks);
                return View("CreateCode", test);
            }
            return View("CreateCode", test);
        }
        public async Task<IActionResult> Details(int? id)
        {
            if (id is null)
            {
                return NotFound();
            }
            //Test test;

            var test = await _testingPlatform.TestManager.FindAsync(t => t.Id == id);

            if (test is null)
            {
                return NotFound();
            }

            return View(test);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Test test)
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
            if (id == null)
            {
                return NotFound();
            }

            var test = await _testingPlatform.TestManager.GetTestAsync(_userId, id);
            if (test == null)
            {
                return RedirectToAction(nameof(Index));
            }
            if (!(test.TestCode is null))
                return NotFound();

            return View(test);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,TestName,CreationDate, TestDuration")] Test test)
        {
            if (id != test.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    test.AppUserId = _userId;
                    await _testingPlatform.TestManager.UpdateAsync(test);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (_testingPlatform.TestManager.GetTestAsync(_userId, id) is null)
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(test);
        }
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var test = await _testingPlatform.TestManager.FindAsync(m => m.Id == id && m.AppUserId == _userId);
            if (test == null)
            {
                return NotFound();
            }

            return View(test);
        }
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var test = await _testingPlatform.TestManager.FindAsync(t => t.Id == id);
            if (test is null)
                return NotFound();

            var testQuestions = await _testingPlatform.TestQuestionManager
                .GetAll()
                .Where(tq => tq.AppUserId == _userId && tq.TestId == id)
                .ToListAsync();

            if (testQuestions is null)
                return NotFound();

            var testAnswers = await _testingPlatform.TestAnswerManager
                .GetAll()
                .Where(ta => ta.AppUserId == _userId && ta.TestQuestion.TestId == id)
                .ToListAsync();

            if (testAnswers is null)
            
            test.TestQuestions = testQuestions;
            test.TestAnswers = testAnswers;
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

            var testQuestions = await _testingPlatform.TestQuestionManager
                .GetAll()
                .Where(ta => ta.AppUserId == _userId && ta.TestId == id)
                .ToListAsync();

            var errorModelTest = new Test();

            if (testQuestions.Count == 0)
            {
                errorModelTest.TestQuestions = new List<TestQuestion>();
                return View(errorModelTest);
            }

            var test = testQuestions.FirstOrDefault().Test;

            if (test.TestQuestions.Any(tq => tq.TestAnswers.Count == 0))
            {
                errorModelTest.TestQuestions = new List<TestQuestion>();
                foreach (var testQuestion in test.TestQuestions.Where(tq => tq.TestAnswers.Count == 0))
                {
                    errorModelTest.TestQuestions.Add(testQuestion);
                }
            }

            if (test.TestQuestions.Any(tq => tq.TestAnswers.All(ta => !ta.IsCorrect)))
            {
                errorModelTest.TestQuestions = new List<TestQuestion>();
                foreach (var testQuestion in test.TestQuestions.Where(tq => tq.TestAnswers.All(ta => !ta.IsCorrect)))
                {
                    errorModelTest.TestQuestions.Add(testQuestion);
                }
            }

            return View(errorModelTest);
        }

        [HttpGet]
        [ActionName("TopRated")]
        public async Task<IActionResult> TopRatedGet(string SearchString)
        {
            List<Test> topRatedTests;
            if (SearchString is null)
                topRatedTests = await _testingPlatform.TestManager
                .GetAll()
                .Where(t => !(t.TestCode == null) && t.TestMarks.Count(tm => tm.EnjoyedTest) - t.TestMarks.Count(tm => !tm.EnjoyedTest) >= 1)
                .Take(Int32.Parse(_config.Value.TakeAmount))
                .OrderByDescending(t => t.TestMarks.Count(tm => tm.EnjoyedTest) - t.TestMarks.Count(tm => !tm.EnjoyedTest))
                .ToListAsync();
            else
                topRatedTests = await _testingPlatform.TestManager
                    .GetAll()
                    .Where(t => !(t.TestCode == null) && t.TestName.Contains(SearchString) && t.TestMarks.Count(tm => tm.EnjoyedTest) - t.TestMarks.Count(tm => !tm.EnjoyedTest) >= 1)
                    .Take(Int32.Parse(_config.Value.TakeAmount))
                    .OrderByDescending(t => t.TestMarks.Count(tm => tm.EnjoyedTest) - t.TestMarks.Count(tm => !tm.EnjoyedTest))
                    .ToListAsync();

            if (topRatedTests is null)
                return NotFound();

            return View(topRatedTests);
        }
        [HttpGet]
        [ActionName("SearchTests")]
        public async Task<IActionResult> SearchTestsGet(string searchString)
        {
            List<Test> tests;
            if (searchString is null)
                tests = await _testingPlatform.TestManager
                    .GetAll()
                    .Where(t => t.TestCode != null)
                    .Take(Int32.Parse(_config.Value.TakeAmount))
                    .ToListAsync();
            else
                tests = await _testingPlatform.TestManager
                    .GetAll()
                    .Where(t => t.TestCode != null && t.TestName.ToUpper().Contains(searchString.ToUpper()))
                    .Take(Int32.Parse(_config.Value.TakeAmount))
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
                .Where(t => t.TestName.Contains(searchString))
                .Skip(skipAmount.Value)
                .Take(amount.Value)
                .ToListAsync();
            var optimizedTests = tests.Select(t =>
            new
            {
                id = t.Id,
                testName = t.TestName,
                creationDate = t.CreationDate,
                testCode = t.TestCode,
                duration = t.TestDuration,
                testRating = t.TestMarks.Count(tm => tm.EnjoyedTest) - t.TestMarks.Count(tm => !tm.EnjoyedTest),
                userId = t.AppUser.Id,
                userName = t.AppUser.UserName
            }
            );
            return Json(optimizedTests);
        }
        [HttpPost]
        [Authorize(Roles = "Admin, Moderator")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GetReportedTestsAjax(int? skipAmount, int? amount, string searchString)
        {
            if (skipAmount is null || amount is null)
                return BadRequest();

            if (searchString is null)
                searchString = "";

            var tests = await _testingPlatform.TestManager
                .GetAll()
                .Where(t => t.TestReports.Count >= 1 && t.TestName.Contains(searchString))
                .Skip(skipAmount.Value)
                .Take(amount.Value)
                .ToListAsync();
            var optimizedTests = tests.Select(t =>
            new
            {
                id = t.Id,
                testName = t.TestName,
                userName = t.AppUser.UserName,
                userId = t.AppUserId,
                testCode = t.TestCode,
                reportAmount = t.TestReports.Count,
                testRating = t.TestMarks.Count(tm => tm.EnjoyedTest) - t.TestMarks.Count(tm => !tm.EnjoyedTest),
            }
            );
            return Json(optimizedTests);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GetSharedTestsAjax(int? skipAmount, int? amount, string searchString)
        {
            if (skipAmount is null || amount is null)
                return BadRequest();

            if (searchString is null)
                searchString = "";

            var tests = await _testingPlatform.TestManager
                .GetAll()
                .Where(t => t.TestCode != null && t.TestName.Contains(searchString))
                .Skip(skipAmount.Value)
                .Take(amount.Value)
                .ToListAsync();
            var optimizedTests = tests.Select(t =>
            new
            {
                id = t.Id,
                testName = t.TestName,
                creationDate = t.CreationDate,
                testCode = t.TestCode,
                duration = t.TestDuration,
                testRating = t.TestMarks.Count(tm => tm.EnjoyedTest) - t.TestMarks.Count(tm => !tm.EnjoyedTest),
                userName = t.AppUser.UserName
            }
            );
            return Json(optimizedTests);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GetTopTestsAjax(int? skipAmount, int? amount, string searchString)
        {
            if (skipAmount is null || amount is null)
                return BadRequest();

            if (searchString is null)
                searchString = "";

            var topRatedTests = await _testingPlatform.TestManager
                .GetAll()
                .Where(t => t.TestCode != null && t.TestName.Contains(searchString) && t.TestMarks.Count(tm => tm.EnjoyedTest) - t.TestMarks.Count(tm => !tm.EnjoyedTest) >= 1)
                .OrderByDescending(t => t.TestMarks.Count(tm => tm.EnjoyedTest) - t.TestMarks.Count(tm => !tm.EnjoyedTest))
                .Skip(skipAmount.Value)
                .Take(amount.Value)
                .ToListAsync();
            
            var optimizedTests = topRatedTests.Select(t =>
            new
            {
                id = t.Id,
                testName = t.TestName,
                creationDate = t.CreationDate,
                duration = t.TestDuration,
                testCode = t.TestCode,
                testRating = t.TestMarks.Count(tm => tm.EnjoyedTest) - t.TestMarks.Count(tm => !tm.EnjoyedTest),
            }
            );
            return Json(optimizedTests);
        }
        private bool HasValidationErrors(int? id)
        {
            if (id is null)
                return true;

            var testQuestions = _testingPlatform.TestQuestionManager.GetAll().Where(tq => tq.TestId == id && tq.AppUserId == _userId).ToList();
            var test = testQuestions.FirstOrDefault()?.Test;

            if (test is null)
                return true;

            if (test.TestQuestions.Count == 0)
            {
                return true;
            }

            if (test.TestQuestions.Any(tq => tq.TestAnswers.Count == 0))
            {
                return true;
            }
            if (test.TestQuestions.Any(tq => tq.TestAnswers.All(ta => !ta.IsCorrect)))
            {
                return true;
            }
            return false;
        }
    }
}
