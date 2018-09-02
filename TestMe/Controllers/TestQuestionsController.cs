using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TestMe.Data;
using TestMe.Models;
using TestMe.Sevices.Interfaces;

namespace TestMe.Controllers
{
    [Authorize]
    public class TestQuestionsController : Controller
    {
        private readonly ITestingPlatform _testingPlatform;
        private readonly UserManager<AppUser> _userManager;
        private string _userId;
        public TestQuestionsController(ITestingPlatform testingPlatform, UserManager<AppUser> userManager)
        {
            _testingPlatform = testingPlatform;
            _userManager = userManager;
        }
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            _userId = _userManager.GetUserId(User);
        }

        public async Task<IActionResult> Index(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index", "Tests");
            }

            var testQuestions = await _testingPlatform.TestQuestionManager.GetAll().Where(t => t.AppUser.Id == _userId && t.TestId == id).ToListAsync();
            var test = testQuestions.FirstOrDefault()?.Test;
            if (test == null)
            {
                return RedirectToAction("Index", "Tests");
            }

            return View(test);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index", "Tests");
            }

            var testQuestion = await _testingPlatform.TestQuestionManager.GetTestQuestionAsync(_userId, id);
            if (testQuestion == null)
            {
                return RedirectToAction("Index", "Tests");
            }

            return View(testQuestion);
        }

        public async Task<IActionResult> Create(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index", "Tests");
            }

            var test = await _testingPlatform.TestManager.GetTestAsync(_userId, id);
            if (test == null)
            {
                return RedirectToAction("Index", "Tests");
            }

            if (!(test.TestCode is null))
                return RedirectToAction("Index", new { id });

            var testQuestion = new TestQuestion { TestId = test.Id, Test = test }; 
            return View(testQuestion);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("QuestionText", "TestId")] TestQuestion testQuestion)
        {
            if (ModelState.IsValid)
            {
                testQuestion.AppUserId = _userId;
                await _testingPlatform.TestQuestionManager.AddAsync(testQuestion);
                return RedirectToAction(nameof(Index), new { id = testQuestion.TestId });
            }
            return View();
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index", "Tests");
            }

            var testQuestion = await _testingPlatform.TestQuestionManager.GetTestQuestionAsync(_userId, id);
            if (testQuestion == null)
            {
                return RedirectToAction("Index", "Tests");
            }
            if (!(testQuestion.Test.TestCode is null))
                return RedirectToAction("Index", new { id });

            return View(testQuestion);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,QuestionText,TestId")] TestQuestion testQuestion)
        {
            if (id != testQuestion.Id)
            {
                return RedirectToAction("Index", "Tests");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    testQuestion.AppUserId = _userId;
                    await _testingPlatform.TestQuestionManager.UpdateAsync(testQuestion);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (_testingPlatform.TestQuestionManager.GetTestQuestionAsync(_userId, id) is null)
                    {
                        return RedirectToAction("Index", "Tests");
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index), new { id = testQuestion.TestId });
            }
            return View(testQuestion);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index", "Tests");
            }

            var testQuestion = await _testingPlatform.TestQuestionManager.GetTestQuestionAsync(_userId, id);
            if (testQuestion == null)
            {
                return RedirectToAction("Index", "Tests");
            }
            if (!(testQuestion.Test.TestCode is null))
                return RedirectToAction("Index", new { id });

            return View(testQuestion);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var testQuestion = await _testingPlatform.TestQuestionManager.GetTestQuestionAsync(_userId, id);
            var testId = testQuestion.TestId;
            foreach (var testAnswer in testQuestion.TestAnswers.Where(ta => !(ta.ImageName is null)))
                _testingPlatform.AnswerImageManager.DeleteAnswerImage(testAnswer.ImageName);

            await _testingPlatform.TestQuestionManager.DeleteAsync(testQuestion);
            return RedirectToAction(nameof(Index), new { id = testId });
        }

    }
}
