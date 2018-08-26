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
using Newtonsoft.Json;

namespace TestMe.Controllers
{
    public class TestEngineController : Controller
    {
        private ApplicationDbContext _context;
        public TestEngineController(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index(string code)
        {
            HttpContext.Session.Clear();
            var test = await GetTestAsync(code);
            if (test is null)
            {
                return RedirectToAction("Index", "Home");
            }
            if(User.Identity.IsAuthenticated)
                HttpContext.Session.SetString("userName", User.Identity.Name);

            HttpContext.Session.SetString("testCode", code);
            HttpContext.Session.SetString("correctlyAnswered", "");
            return View(test);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SetUserName(string userName)
        {
            if (!(HttpContext.Session.GetString("userName") is null))
                return Json("success");

           if (userName is null)
                    return Json("error");

            HttpContext.Session.SetString("userName", userName);

            return Json("success");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult GetUserName()
        {
            var userName = HttpContext.Session.GetString("userName");

            if (userName is null)
                throw new Exception("No user matching user was found");

            return Json(userName);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> StartTest()
        {
            var testCode = HttpContext.Session.GetString("testCode");

            if (testCode is null)
                return View("Index", "Home");

            var testAnswers = await GetTestAnswersAsync(testCode);

            if (testAnswers is null)
                return RedirectToAction("Index", "Home");

            //if (testAnswers.TestQuestions.Any(t => !(t.TestAnswers is null)))
            //    return Json("Error");

            return Json(testAnswers.FirstOrDefault().TestQuestion);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GetIfAlreadyAnswered(int? questionId)
        {
            if (questionId is null)
                return Json("error");

            if (!(HttpContext.Session.GetString(questionId.ToString()) is null))
            {
                var userAnswersStr = HttpContext.Session.GetString(questionId.ToString());
                var userAnswers = JsonConvert.DeserializeObject<List<int>>(userAnswersStr);
                return Json(userAnswers);
            }

            return Json("error");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CheckAnswer(int? questionId, List<int> checkedIds)
        {
            var testCode = HttpContext.Session.GetString("testCode");

            if (testCode is null)
                return View("Index", "Home");

            if (questionId is null || checkedIds is null)
                return Json("error");


            var testAnswers = await GetTestAnswersAsync(testCode, questionId);

            if (testAnswers is null)
                return RedirectToAction("Index", "Home");

            var _answers = testAnswers.Where(ta => ta.TestQuestionId == questionId);


            if (_answers.Count(ta => ta.TestQuestionId == questionId) == 0)
                return Json("error");

            if (!(HttpContext.Session.GetString(questionId.ToString()) is null))
            {
                var correctAnswers = new List<int>();
                foreach (var correct in _answers.Where(ta => ta.IsCorrect))
                {
                    correctAnswers.Add(correct.Id);
                }
                return Json(correctAnswers);
            }

            if (checkedIds.Count == 0)
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
                    break;
                }
            }
            if (isCorrect)
            {
                var alreadyCorrectlyAnsweredStr = HttpContext.Session.GetString("correctlyAnswered");
                var alreadyCorrectlyAnswered = JsonConvert.DeserializeObject<List<int>>(alreadyCorrectlyAnsweredStr);
                if (alreadyCorrectlyAnswered is null)
                    alreadyCorrectlyAnswered = new List<int>();
                alreadyCorrectlyAnswered.Add(questionId.Value);
                HttpContext.Session.SetString("correctlyAnswered", JsonConvert.SerializeObject(alreadyCorrectlyAnswered));
            }
            var serializedAnswers = JsonConvert.SerializeObject(checkedIds);
            HttpContext.Session.SetString(questionId.ToString(), serializedAnswers);

            var answers = new List<int>();
            foreach(var correct in _answers.Where(ta => ta.IsCorrect))
            {
                answers.Add(correct.Id);
            }
            return  Json(answers);

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GetQuestionsIds()
        {
            var testCode = HttpContext.Session.GetString("testCode");

            if (testCode is null)
                return View("Index", "Home");

            var testAnswers = await GetTestAnswersAsync(testCode);

            if (testAnswers is null)
                return RedirectToAction("Index", "Home");

            var firstAnswer = testAnswers.FirstOrDefault();
            if (firstAnswer is null)
                return Json("Error");

            var questionIds = new List<int>();
            foreach(var question in firstAnswer.TestQuestion.Test.TestQuestions)
            {
                questionIds.Add(question.Id);
            }

            return Json(questionIds);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GetNextQuestion(int? questionId)
        {
            var testCode = HttpContext.Session.GetString("testCode");

            if (testCode is null)
                return View("Index", "Home");

            if (questionId is null)
                return Json("error");

            var testAnswers = await GetTestAnswersAsync(testCode);
            var nextQuestion = testAnswers.SkipWhile(ta => ta.TestQuestionId != questionId)
                .SkipWhile(ta => ta.TestQuestionId == questionId)
                .FirstOrDefault()?
                .TestQuestion;

            if (nextQuestion is null)
            {

                if (testAnswers.Any(ta => ta.TestQuestionId == questionId))
                    return Json("bound");
                else
                    return Json("error");
            }

            return Json(nextQuestion);

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GetPrevQuestion(int? questionId)
        {
            var testCode = HttpContext.Session.GetString("testCode");

            if (testCode is null)
                return View("Index", "Home");

            if (questionId is null)
                return Json("error");

            var testAnswers = await GetTestAnswersAsync(testCode);
            testAnswers = testAnswers.Reverse();
            var prevQuestion = testAnswers.SkipWhile(ta => ta.TestQuestionId != questionId)
                .SkipWhile(ta => ta.TestQuestionId == questionId)
                .FirstOrDefault()?
                .TestQuestion;

            if (prevQuestion is null)
            {

                if (testAnswers.Any(ta => ta.TestQuestionId == questionId))
                    return Json("bound");
                else
                    return Json("error");
            }

            return Json(prevQuestion);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GetQuestion(int? questionId)
        {
            var testCode = HttpContext.Session.GetString("testCode");

            if (testCode is null)
                return View("Index", "Home");

            if (questionId is null)
                return Json("error");

            var testAnswers = await GetTestAnswersAsync(testCode);
            var question = testAnswers
                .FirstOrDefault(ta => ta.TestQuestionId == questionId)?
                .TestQuestion;

            if (question is null)
                return Json("error");

            return Json(question);
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