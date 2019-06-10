using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using TestMe.Models;
using Newtonsoft.Json;
using TestMe.Exceptions;
using TestMe.Sevices.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using TestMe.ViewModels;

namespace TestMe.Controllers
{
    [Authorize]
    public class TestEngineController : Controller
    {
        private readonly ITestingPlatform _testingPlatform;
        private readonly UserManager<AppUser> _userManager;

        public TestEngineController(ITestingPlatform testingPlatform, UserManager<AppUser> userManager)
        {
            _testingPlatform = testingPlatform;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index(string code)
        {

            var test = await _testingPlatform.TestManager
                .FindAsync(t => t.TestCode == code);
            if (test is null)
                throw new TestNotFoundException(code);

            var testReports = await _testingPlatform.TestReportManager
                .GetAll()
                .Where(tr => tr.TestId == test.Id && tr.AppUserId == _userManager.GetUserId(User))
                .ToListAsync();

            if (testReports is null)
                return NotFound();

            var userResult = await _testingPlatform.TestResultManager
                .GetAll()
                .FirstOrDefaultAsync(tr => tr.AppUser.UserName == User.Identity.Name && tr.TestId == test.Id);

            var testResults = await _testingPlatform.TestResultManager
                .GetAll()
                .Where(tr => tr.TestId == test.Id)
                .ToListAsync();


            var testViewModel = new TestEngineViewModel();
                //test.TestResults = new List<TestResult> { userResult };
            

            test.TestReports = testReports;
            test.TestResults = testResults;

            testViewModel.Test = test;
            testViewModel.UserTestResult = userResult;


            var testCode = HttpContext.Session.GetString("testCode");
            if (HttpContext.Session.GetString("endTime") is null || testCode is null || (!(testCode is null) && testCode != code))
                HttpContext.Session.Clear();

            HttpContext.Session.SetString("testCode", code);

            return View(testViewModel);
        }
        public override void OnActionExecuting(ActionExecutingContext context)
        {

            if (context.RouteData.Values["action"].ToString() != "Index" &&
                context.RouteData.Values["action"].ToString() != "FinishTest" &&
                context.RouteData.Values["action"].ToString() != "RateFinishedTestAjax" &&
                context.RouteData.Values["action"].ToString() != "StartTest" &&
                context.RouteData.Values["action"].ToString() != "GetCorrectAnswers")
            {
                var startTimeStr = HttpContext.Session.GetString("startTime");
                var endTimeStr = HttpContext.Session.GetString("endTime");

                if (!(startTimeStr is null) && !(endTimeStr is null))
                {
                    var endTime = JsonConvert.DeserializeObject<DateTime>(endTimeStr);
                    if (DateTime.Compare(endTime.AddSeconds(1), DateTime.Now) < 0)
                    {
                        FinishTest().GetAwaiter().GetResult();
                    }
                }

            }
        }

        private async Task DeleteOldAnswerAsync()
        {
            var testCode = HttpContext.Session.GetString("testCode");
            if (testCode is null)
                throw new TestNotFoundException();

            var userId = _userManager.GetUserId(User);
            var oldAnswers = _testingPlatform.UserAnswerManager
                .GetAll()
                .Where(ua =>
                ua.AppUserId == userId && 
                ua.TestAnswer.TestQuestion.Test.TestCode == testCode);

            await _testingPlatform.UserAnswerManager.DeleteRangeAsync(oldAnswers);
        }

        [HttpPost]
        public IActionResult GetUserName()
        {
            return Json(User.Identity.Name);
        }

        [HttpPost]
        public async Task<IActionResult> StartTest()
        {
            var testCode = HttpContext.Session.GetString("testCode");

            if (testCode is null)
                throw new TestNotFoundException();

            var testQuestion = await _testingPlatform.TestQuestionManager
                .GetAll()
                .FirstOrDefaultAsync(tq => tq.Test.TestCode == testCode);

            if (testQuestion is null)
                throw new QuestionNotFoundException();

            if (HttpContext.Session.GetString("startTime") is null)
            {
                var currentTimeSerialized = JsonConvert.SerializeObject(DateTime.Now);
                HttpContext.Session.SetString("startTime", currentTimeSerialized);
            }

            if (HttpContext.Session.GetString("endTime") is null)
            {
                var endTime = DateTime.Now + testQuestion.Test.TestDuration;
                var endTimeSerialized = JsonConvert.SerializeObject(endTime);
                HttpContext.Session.SetString("endTime", endTimeSerialized);
            }

            await DeleteOldAnswerAsync();

            if (HttpContext.Session.GetString("answeredQuestions") is null)
                HttpContext.Session.SetString("answeredQuestions", JsonConvert.SerializeObject(new Dictionary<int, bool>()));

            return Json(testQuestion);
        }

        [HttpPost]
        public IActionResult GetIfAnswered(int? questionId)
        {
            if (questionId is null)
                throw new QuestionNotFoundException();

            if (!(HttpContext.Session.GetString(questionId.ToString()) is null))
            {
                var answers = JsonConvert.DeserializeObject<List<int>>(HttpContext.Session.GetString(questionId.ToString()));
                return Json(answers);
            }
            return Json("notAnswered");
        }

        [HttpPost]
        public async Task<IActionResult> CheckAnswer(int? questionId, List<int> checkedIds)
        {
            var testCode = HttpContext.Session.GetString("testCode");

            if (testCode is null)
                throw new TestNotFoundException();

            if (questionId is null)
                throw new QuestionNotFoundException();

            if (checkedIds is null)
                throw new UserAnswersException();

            if (checkedIds.Count == 0)
                throw new UserAnswersException();

            var questionAnswers = _testingPlatform.TestAnswerManager
                .GetAll()
                .Where(qa =>
                qa.TestQuestion.Test.TestCode == testCode &&
                qa.TestQuestionId == questionId);
           
            if(checkedIds.Except(questionAnswers.Select(qa => qa.Id)).Count() != 0)
                throw new UserAnswersException();

            var _answers = questionAnswers.Where(qa => qa.IsCorrect).ToList();

            if (_answers.Count == 0)
                throw new QuestionNotFoundException(questionId.ToString());

            bool isCorrect = true;
            var correctAnswers = _answers.Select(ta => ta.Id);
            if (correctAnswers.Except(checkedIds).Any() || checkedIds.Except(correctAnswers).Any())
                isCorrect = false;

            var answeredQuestionsStr = HttpContext.Session.GetString("answeredQuestions");
            if (answeredQuestionsStr is null)
                return NotFound();

            var answeredQuestions = JsonConvert.DeserializeObject<Dictionary<int, bool>>(answeredQuestionsStr);
            answeredQuestions[questionId.Value] = isCorrect;
            HttpContext.Session.SetString("answeredQuestions", JsonConvert.SerializeObject(answeredQuestions));
            var userId = _userManager.GetUserId(User);
            foreach (var answer in checkedIds)
            {
                var prevAnswer = await _testingPlatform.UserAnswerManager.FindAsync(ua =>
                ua.AppUserId == userId &&
                ua.TestAnswerId == answer);
                if (prevAnswer is null)
                {
                    var userAnswer = new UserAnswer {
                        TestAnswerId = answer,
                        AppUserId = userId,
                        AnswerTime = DateTime.Now
                    };
                    await _testingPlatform.UserAnswerManager.AddAsync(userAnswer);
                }
                else
                {
                    prevAnswer.AnswerTime = DateTime.Now;
                    await _testingPlatform.UserAnswerManager.UpdateAsync(prevAnswer);
                }

            }
            var serializedAnswers = JsonConvert.SerializeObject(checkedIds);
            HttpContext.Session.SetString(questionId.ToString(), serializedAnswers);

            return Json("success");

        }

        [HttpPost]
        public async Task<IActionResult> GetQuestionsIds()
        {
            var testCode = HttpContext.Session.GetString("testCode");

            if (testCode is null)
                throw new TestNotFoundException();

            var questions = await _testingPlatform.TestQuestionManager
                .GetAll()
                .Where(tq => tq.Test.TestCode == testCode)
                .ToListAsync();

            return Json(questions.Select(tq => 
            new {
                tq.Id,
                answered = !(HttpContext.Session.GetString(tq.Id.ToString()) is null)
            }));
        }

        [HttpPost]
        public async Task<IActionResult> GetQuestion(int? questionId)
        {
            var testCode = HttpContext.Session.GetString("testCode");

            if (testCode is null)
                throw new TestNotFoundException();

            if (questionId is null)
                throw new QuestionNotFoundException();

            var question = await _testingPlatform.TestQuestionManager
                .GetAll()
                .FirstOrDefaultAsync(tq => tq.Test.TestCode == testCode && tq.Id == questionId);

            if (question is null)
                throw new QuestionNotFoundException();

            return Json(question);
        }

        [HttpPost]
        public IActionResult GetEndTime(string code)
        {
            var codeFromSession = HttpContext.Session.GetString("testCode");
            if (!(codeFromSession is null) && codeFromSession != code)
                throw new TestTimeException();

            var endTimeStr = HttpContext.Session.GetString("endTime");
            if (endTimeStr is null)
                throw new TestTimeException();

            var endTime = JsonConvert.DeserializeObject<DateTime>(endTimeStr);
            return Json(endTime);
        }

        [HttpPost]
        public async Task<IActionResult> FinishTest()
        {
            var alreadyAnsweredStr = HttpContext.Session.GetString("answeredQuestions");
            var alreadyAnswered = JsonConvert.DeserializeObject<Dictionary<int, bool>>(alreadyAnsweredStr);
            int score = alreadyAnswered.Values.Count(v => v);

            var code = HttpContext.Session.GetString("testCode");
            if (code is null)
                throw new TestNotFoundException();

            var test = await _testingPlatform.TestManager.FindAsync(t => t.TestCode == code);
            if (test is null)
                throw new TestNotFoundException(code);

            var prevResult = await _testingPlatform.TestResultManager
                .FindAsync(tr => tr.AppUserId == _userManager.GetUserId(User) && tr.TestId == test.Id);

            if (prevResult is null && HttpContext.Session.GetString("isFinished") is null)
            {
                var startTimeStr = HttpContext.Session.GetString("startTime");
                if (startTimeStr is null)
                    throw new TestTimeException();

                var endTimeStr = HttpContext.Session.GetString("endTime");
                if (endTimeStr is null)
                    throw new TestTimeException();

                var startTime = JsonConvert.DeserializeObject<DateTime>(startTimeStr);
                var endTime = DateTime.Now;
                var endTestTime = JsonConvert.DeserializeObject<DateTime>(endTimeStr);
                if (DateTime.Compare(endTestTime, DateTime.Now) < 0)
                    endTime = endTestTime;

                var user = await _userManager.FindByNameAsync(User.Identity.Name);
                var testResult = new TestResult { AppUserId = user.Id, Score = score, TestId = test.Id, StartTime = startTime, FinishTime = endTime };
                await _testingPlatform.TestResultManager.AddAsync(testResult);
            }
            HttpContext.Session.SetString("isFinished", "true");

            var prevMark = await _testingPlatform.TestMarkManager
                .FindAsync(tm => tm.AppUserId == _userManager.GetUserId(User) && tm.TestId == test.Id);

            HttpContext.Session.Remove("endTime");
            HttpContext.Session.Remove("startTime");

            if (prevMark is null)
                return Json(new { score, testId = test.Id });

            return Json(new { score, testId = test.Id, isRated = prevMark.EnjoyedTest });
        }

        [HttpPost]
        public async Task<IActionResult> GetCorrectAnswers()
        {
            var code = HttpContext.Session.GetString("testCode");
            if (code is null)
                throw new TestNotFoundException();

            var test = await _testingPlatform.TestManager.FindAsync(t => t.TestCode == code);
            if (test is null)
                throw new TestNotFoundException(code);

            if (HttpContext.Session.GetString("isFinished") is null)
                return NotFound();

                var optimizedQuestions = test.TestQuestions.Select(tq => new
            {
                tq.Id,
                tq.QuestionText,
                tq.PreformattedText,
                testAnswers = tq.TestAnswers.Select(ta => new { ta.Id, ta.AnswerText, ta.ImageName, ta.IsCorrect, ta.IsCode }),
                userAnswers = JsonConvert
                           .DeserializeObject<List<int>>(HttpContext.Session.GetString(tq.Id.ToString()) ?? "")
            });
            return Json(optimizedQuestions);

        }

        [HttpPost]
        public async Task<IActionResult> RateFinishedTestAjax(int? id, bool? mark)
        {
            if (mark is null)
                return NotFound();

            if (id is null)
            {
                var code = HttpContext.Session.GetString("testCode");
                if (code is null)
                    throw new TestNotFoundException();

                id = (await _testingPlatform.TestManager.FindAsync(t => t.TestCode == code)).Id;

            }

            var testResult = await _testingPlatform.TestResultManager
                .FindAsync(tr => tr.AppUserId == _userManager.GetUserId(User) && tr.TestId == id);

            if (testResult is null)
                return NotFound();

            var test = await _testingPlatform.TestManager.FindAsync(t => t.Id == id);
            var prevMark = await _testingPlatform.TestMarkManager
                .FindAsync(tm => tm.AppUserId == _userManager.GetUserId(User) && tm.TestId == test.Id);
            if (prevMark is null)
            {
                var testMark = new TestMark { TestId = test.Id, AppUserId = _userManager.GetUserId(User), EnjoyedTest = mark.Value };
                await _testingPlatform.TestMarkManager.AddAsync(testMark);
            }
            else
            {
                prevMark.EnjoyedTest = mark.Value;
                await _testingPlatform.TestMarkManager.UpdateAsync(prevMark);
            }
            return Json(mark);
        }
    }
}