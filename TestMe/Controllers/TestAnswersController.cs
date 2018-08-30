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
    public class TestAnswersController : Controller
    {
        private readonly ITestingPlatform _testingPlatform;
        private readonly UserManager<AppUser> _userManager;
        private string _userId;
        public TestAnswersController(ITestingPlatform testingPlatform, UserManager<AppUser> userManager)
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
                return NotFound();
            }

            var testQuestion = await _testingPlatform.TestQuestionManager.GetTestQuestionAsync(_userId, id);
            if (testQuestion == null)
            {
                return NotFound();
            }
            ViewBag.TestId = testQuestion.TestId;
            ViewBag.TestQuestionId = testQuestion.Id;
            ViewBag.TestQuestionText = testQuestion.QuestionText;
            var testAnswers = _testingPlatform.TestAnswerManager.GetAll().Where(ta => ta.TestQuestionId == id && ta.AppUserId == _userId);
            return View(testAnswers);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var testAnswer = await _testingPlatform.TestAnswerManager.GetTestAnswerAsync(_userId, id);
            if (testAnswer == null)
            {
                return NotFound();
            }

            return View(testAnswer);
        }

        public async Task<IActionResult> Create(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var testQuestion = await _testingPlatform.TestQuestionManager.GetTestQuestionAsync(_userId, id);
            if (testQuestion == null)
            {
                return NotFound();
            }
            ViewBag.TestQuestionId = testQuestion.Id;
            ViewBag.TestQuestionText = testQuestion.QuestionText;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("AnswerText,IsCorrect,TestQuestionId")] TestAnswer testAnswer)
        {
            if (ModelState.IsValid)
            {
                testAnswer.AppUserId = _userId;
                await _testingPlatform.TestAnswerManager.AddAsync(testAnswer);
                return RedirectToAction(nameof(Index), new { id = testAnswer.TestQuestionId });
            }
            return View(testAnswer);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var testAnswer = await _testingPlatform.TestAnswerManager.GetTestAnswerAsync(_userId, id);
            if (testAnswer == null)
            {
                return NotFound();
            }
            ViewBag.TestQuestionId = testAnswer.TestQuestionId;
            ViewBag.TestQuestionText = testAnswer.TestQuestion.QuestionText;
            return View(testAnswer);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id, AnswerText,IsCorrect,TestQuestionId")] TestAnswer testAnswer)
        {
            if (id != testAnswer.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    testAnswer.AppUserId = _userId;
                    await _testingPlatform.TestAnswerManager.UpdateAsync(testAnswer);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (await _testingPlatform.TestAnswerManager.GetTestAnswerAsync(_userId, id) is null)
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index), new { id = testAnswer.TestQuestionId });
            }
            return View(testAnswer);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var testAnswer = await _testingPlatform.TestAnswerManager.GetTestAnswerAsync(_userId, id);
            if (testAnswer == null)
            {
                return NotFound();
            }

            return View(testAnswer);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var testAnswer = await _testingPlatform.TestAnswerManager.GetTestAnswerAsync(_userId, id);
            await _testingPlatform.TestAnswerManager.DeleteAsync(testAnswer);
            return RedirectToAction(nameof(Index), new { id = testAnswer.TestQuestionId });
        }

    }
}
