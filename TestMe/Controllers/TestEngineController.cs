using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using TestMe.Data;
using TestMe.Models;

namespace TestMe.Controllers
{
    public class TestEngineController : Controller
    {
        private ApplicationDbContext _context;
        private Dictionary<TestQuestion, bool> _correctllyAnswered;
        public TestEngineController(ApplicationDbContext context)
        {
            _context = context;
            _correctllyAnswered = new Dictionary<TestQuestion, bool>();
        }
        public async Task<IActionResult> Index(string code)
        {
            var test = await GetTestAsync(code);
            if (test is null)
            {
                return RedirectToAction("Index", "Home");
            }
            HttpContext.Session.SetString("testCode", code);
            return View(test);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> StartTest()
        {
            var testCode = HttpContext.Session.GetString("testCode");
            var testAnswers = await GetTestAnswersAsync(testCode);

            if (testAnswers is null)
                return RedirectToAction("Index", "Home");


            //if (testAnswers.TestQuestions.Any(t => !(t.TestAnswers is null)))
            //    return Json("Error");

            return Json(testAnswers.FirstOrDefault().TestQuestion);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CheckAnswer(int? questionId, params int[] checkedIds)
        {
            var testCode = HttpContext.Session.GetString("testCode");
            if (questionId is null || testCode is null || checkedIds is null || checkedIds.Length == 0 )
                return Json("error");

            var testAnswers = await GetTestAnswersAsync(testCode, questionId);

            if (testAnswers is null)
                return RedirectToAction("Index", "Home");

            var _answers = testAnswers.Where(ta => ta.TestQuestionId == questionId);
            if (_answers.Count(ta => ta.TestQuestionId == questionId) == 0)
                return Json("error");

            if (checkedIds.Any(checkId => _answers.Count(ta => ta.Id == checkId) == 0))
                return Json("error");

            var question = await _context.TestQuestions.FirstOrDefaultAsync(tq => tq.Id == questionId);
            bool isCorrect = true;
            foreach(var answId in checkedIds)
            {
                if (_answers.FirstOrDefault(ta => ta.Id == answId && ta.IsCorrect) is null)
                {
                    isCorrect = false;
                    _correctllyAnswered[question] = false;
                    break;
                }
            }
            if(isCorrect)
                _correctllyAnswered[question] = true;
            var answers = new List<int>();
            foreach(var correct in _answers.Where(ta => ta.IsCorrect))
            {
                answers.Add(correct.Id);
            }
            return  Json(answers);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GetNextQuestion(int? questionId)
        {
            var testCode = HttpContext.Session.GetString("testCode");
            if (questionId is null || testCode is null)
                return Json("error");

            var testAnswers = await GetTestAnswersAsync(testCode);
            var nextQuestion = testAnswers.SkipWhile(ta => ta.TestQuestionId != questionId)
                .SkipWhile(ta => ta.TestQuestionId == questionId)
                .FirstOrDefault()?
                .TestQuestion;

            if (nextQuestion is null)
                return Json("error");

            return Json(nextQuestion);

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GetPrevQuestion(int? questionId)
        {
            var testCode = HttpContext.Session.GetString("testCode");
            if (questionId is null || testCode is null)
                return Json("error");

            var testAnswers = await GetTestAnswersAsync(testCode);
            testAnswers = testAnswers.Reverse();
            var prevQuestion = testAnswers.SkipWhile(ta => ta.TestQuestionId != questionId)
                .SkipWhile(ta => ta.TestQuestionId == questionId)
                .FirstOrDefault()?
                .TestQuestion;

            if (prevQuestion is null)
                return Json("error");

            return Json(prevQuestion);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GetQuestion(int questionId)
        {
            throw new NotImplementedException();
        }
        private async Task<Test> GetTestAsync(string code)
        {
            if (code is null)
            {
                return null;
            }
            var test = await _context.Tests.Include(t => t.AppUser).Include(t => t.TestQuestions).FirstOrDefaultAsync(t => t.TestCode == code);
            return test;
        }
        private async Task<IEnumerable<TestAnswer>> GetTestAnswersAsync(string code)
        {
            if (code is null)
            {
                return null;
            }
            var testAnswers = _context.TestAnswers.Include(t => t.AppUser).Include(t => t.TestQuestion).ThenInclude(t => t.Test).Where(t => t.TestQuestion.Test.TestCode == code);
            return await testAnswers?.ToListAsync();
        }
        private async Task<IEnumerable<TestAnswer>> GetTestAnswersAsync(string code, int? questionId)
        {
            if (code is null || questionId is null)
            {
                return null;
            }
            var testAnswers = _context.TestAnswers
                .Include(t => t.AppUser)
                .Include(t => t.TestQuestion)
                .ThenInclude(t => t.Test)
                .Where(t => t.TestQuestion.Test.TestCode == code && t.TestQuestionId == questionId);

            return await testAnswers?.ToListAsync();
        }
    }
}