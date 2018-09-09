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
using TestMe.Exceptions;
using TestMe.Sevices.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

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
            HttpContext.Session.Clear();
            var test = await _testingPlatform.TestManager.FindAsync(t => t.TestCode == code);
            if (test is null)
                throw new TestNotFoundException(code);
            var testReports = await _testingPlatform.TestReportManager.GetAll().Where(tr => tr.TestId == test.Id && tr.AppUserId == _userManager.GetUserId(User)).ToListAsync();
            if (testReports is null)
                return NotFound();
            test.TestReports = testReports;
            HttpContext.Session.SetString("testCode", code);
            return View(test);
        }
        public override void OnActionExecuting(ActionExecutingContext context)
        {

            if (context.RouteData.Values["action"].ToString() != "Index" &&
                context.RouteData.Values["action"].ToString() != "FinishTest" &&
                context.RouteData.Values["action"].ToString() != "RateFinishedTestAjax")
            {
                var startTimeStr = HttpContext.Session.GetString("startTime");
                var endTimeStr = HttpContext.Session.GetString("endTime");

                if (context.RouteData.Values["action"].ToString() != "StartTest" &&
                    startTimeStr is null)
                    throw new TestTimeException();

                if (!(startTimeStr is null) && !(endTimeStr is null))
                {
                    var endTime = JsonConvert.DeserializeObject<DateTime>(endTimeStr);
                    if (DateTime.Compare(endTime.AddSeconds(1), DateTime.Now) < 0)
                    {
                        FinishTest().GetAwaiter().GetResult();
                        throw new TestTimeException();
                    }
                }

            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult GetUserName()
        {
            return Json(User.Identity.Name);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> StartTest()
        {
            var testCode = HttpContext.Session.GetString("testCode");

            if (testCode is null)
                throw new TestNotFoundException();

            var testAnswers = await _testingPlatform.TestAnswerManager.GetAll().Where(t => t.TestQuestion.Test.TestCode == testCode).ToListAsync();

            if (testAnswers is null)
                throw new AnswerNotFoundException();

            if (!(HttpContext.Session.GetString("startTime") is null))
                throw new TestTimeException(testCode);

            var currentTime = DateTime.Now;
            var currentTimeSerialized = JsonConvert.SerializeObject(currentTime);
            HttpContext.Session.SetString("startTime", currentTimeSerialized);
                
            var endTime = currentTime + testAnswers.FirstOrDefault().TestQuestion.Test.TestDuration;
            var endTimeSerialized = JsonConvert.SerializeObject(endTime);
            HttpContext.Session.SetString("endTime", endTimeSerialized);
            HttpContext.Session.SetString("answeredQuestions", JsonConvert.SerializeObject(new Dictionary<int, bool>()));
            return Json(testAnswers.FirstOrDefault().TestQuestion);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult GetIfAlreadyAnswered(int? questionId)
        {
            if (questionId is null)
                throw new QuestionNotFoundException();

            if (!(HttpContext.Session.GetString(questionId.ToString()) is null))
            {
                var userAnswersStr = HttpContext.Session.GetString(questionId.ToString());
                var userAnswers = JsonConvert.DeserializeObject<List<int>>(userAnswersStr);
                return Json(userAnswers);
            }

            return Json("notAnswered");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CheckAnswer(int? questionId, List<int> checkedIds)
        {
            var testCode = HttpContext.Session.GetString("testCode");

            if (testCode is null)
                throw new TestNotFoundException();

            if (questionId is null)
                throw new QuestionNotFoundException();

            if (checkedIds is null)
                throw new UserAnswersException();


            var testAnswers = _testingPlatform.TestAnswerManager.GetAll().Where(t => t.TestQuestion.Test.TestCode == testCode);

            if (testAnswers is null)
                throw new TestNotFoundException(testCode);

            var _answers = testAnswers.Where(ta => ta.TestQuestionId == questionId);


            if (_answers.Count(ta => ta.TestQuestionId == questionId) == 0)
                throw new QuestionNotFoundException(questionId.ToString());

            if (!(HttpContext.Session.GetString(questionId.ToString()) is null))
            {
                var correctAnswersList = new List<int>();
                foreach (var correct in _answers.Where(ta => ta.IsCorrect))
                {
                    correctAnswersList.Add(correct.Id);
                }
                return Json(correctAnswersList);
            }

            if (checkedIds.Count == 0)
                throw new UserAnswersException();

            if (checkedIds.Any(checkId => _answers.Count(ta => ta.Id == checkId) == 0))
                throw new UserAnswersException();

            var question = await _testingPlatform.TestQuestionManager.FindAsync(tq => tq.Id == questionId);
            bool isCorrect = true;
            var correctAnswers = _answers.Where(ta => ta.IsCorrect).Select(ta => ta.Id);
            if (correctAnswers.Except(checkedIds).Count() != 0 || correctAnswers.Count() != checkedIds.Count)
                isCorrect = false;

            var answeredQuestionsStr = HttpContext.Session.GetString("answeredQuestions");
            var answeredQuestions = JsonConvert.DeserializeObject<Dictionary<int, bool>>(answeredQuestionsStr);
            if (!answeredQuestions.Keys.Contains(questionId.Value))
            {
                 answeredQuestions[questionId.Value] = isCorrect;
            }
            else
                throw new AnswerNotFoundException();

            HttpContext.Session.SetString("answeredQuestions", JsonConvert.SerializeObject(answeredQuestions));

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
                throw new TestNotFoundException();

            var testAnswers = await _testingPlatform.TestAnswerManager.GetAll().Where(t => t.TestQuestion.Test.TestCode == testCode).ToListAsync();

            if (testAnswers is null)
                throw new AnswerNotFoundException();

            var firstAnswer = testAnswers.FirstOrDefault();
            if (firstAnswer is null)
                 throw new AnswerNotFoundException();

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
                throw new TestNotFoundException();

            if (questionId is null)
                throw new QuestionNotFoundException();

            var testAnswers = await _testingPlatform.TestAnswerManager.GetAll().Where(t => t.TestQuestion.Test.TestCode == testCode).ToListAsync();
            var nextQuestion = testAnswers.SkipWhile(ta => ta.TestQuestionId != questionId)
                .SkipWhile(ta => ta.TestQuestionId == questionId)
                .FirstOrDefault()?
                .TestQuestion;

            if (nextQuestion is null)
                throw new QuestionNotFoundException();

            return Json(nextQuestion);

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GetPrevQuestion(int? questionId)
        {
            var testCode = HttpContext.Session.GetString("testCode");

            if (testCode is null)
                throw new TestNotFoundException();

            if (questionId is null)
                throw new QuestionNotFoundException();

            var testAnswers = await _testingPlatform.TestAnswerManager.GetAll().Where(t => t.TestQuestion.Test.TestCode == testCode).ToListAsync();
            testAnswers.Reverse();
            var prevQuestion = testAnswers.SkipWhile(ta => ta.TestQuestionId != questionId)
                .SkipWhile(ta => ta.TestQuestionId == questionId)
                .FirstOrDefault()?
                .TestQuestion;

            if (prevQuestion is null)
                throw new QuestionNotFoundException();

            return Json(prevQuestion);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GetQuestion(int? questionId)
        {
            var testCode = HttpContext.Session.GetString("testCode");

            if (testCode is null)
                throw new TestNotFoundException();

            if (questionId is null)
                throw new QuestionNotFoundException();

            var testAnswers = await _testingPlatform.TestAnswerManager.GetAll().Where(t => t.TestQuestion.Test.TestCode == testCode).ToListAsync();
            var question = testAnswers
                .FirstOrDefault(ta => ta.TestQuestionId == questionId)?
                .TestQuestion;

            if (question is null)
                throw new QuestionNotFoundException();

            return Json(question);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult GetEndTime()
        {
            var endTimeStr = HttpContext.Session.GetString("endTime");
            if (endTimeStr is null)
                throw new TestTimeException();

            var endTime = JsonConvert.DeserializeObject<DateTime>(endTimeStr);
            return Json(endTime);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> FinishTest()
        {
            var alreadyAnsweredStr = HttpContext.Session.GetString("answeredQuestions");
            var alreadyAnswered = JsonConvert.DeserializeObject<Dictionary<int, bool>>(alreadyAnsweredStr);
            int score = alreadyAnswered.Values.Where(v => v).Count();

            var code = HttpContext.Session.GetString("testCode");
            if (code is null)
                throw new TestNotFoundException();

            var test = await _testingPlatform.TestManager.FindAsync(t => t.TestCode == code);
            if (test is null)
                throw new TestNotFoundException(code);

            if (!(HttpContext.Session.GetString("isFinished") is null))
                return Json(new { score, testId = test.Id });

            var prevMark = await _testingPlatform.TestMarkManager.GetAll()
                .FirstOrDefaultAsync(tm => tm.AppUserId == _userManager.GetUserId(User) && tm.TestId == test.Id);
            var prevResult = await _testingPlatform.TestResultManager.GetAll() 
                .FirstOrDefaultAsync(tr => tr.AppUser.Id == _userManager.GetUserId(User) && tr.TestId == test.Id);
            if (prevResult is null)
            {
                var startTimeStr = HttpContext.Session.GetString("startTime");
                if (startTimeStr is null)
                    throw new TestTimeException();
                var endTimeStr = HttpContext.Session.GetString("endTime");

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
            if (prevMark is null)
            {
                return Json(new { score, testId = test.Id });
            }
            else
            {
                return Json(new { score, testId = test.Id, isRated = prevMark.EnjoyedTest });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RateTest(bool? mark)
        {
            if (mark is null)
                return NotFound();

            var code = HttpContext.Session.GetString("testCode");
            if (code is null)
                throw new TestNotFoundException();

            var test = await _testingPlatform.TestManager.FindAsync(t => t.TestCode == code);
            if (test is null)
                throw new TestNotFoundException(code);

            if (HttpContext.Session.GetString("isFinished") is null)
                return NotFound();

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
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RateFinishedTestAjax(int? id, bool? mark)
        {
            if (id is null || mark is null)
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