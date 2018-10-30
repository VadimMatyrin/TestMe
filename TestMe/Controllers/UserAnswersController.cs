using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TestMe.Models;
using TestMe.Sevices.Interfaces;

namespace TestMe.Controllers
{
    public class UserAnswersController : Controller
    {
        private readonly ITestingPlatform _testingPlatform;
        private readonly UserManager<AppUser> _userManager;
        public UserAnswersController(ITestingPlatform testingPlatform, UserManager<AppUser> userManager)
        {
            _testingPlatform = testingPlatform;
            _userManager = userManager;
        }
        public async Task<IActionResult> Index(int? testId, string userId)
        {
            if (testId is null)
                return NotFound();

            if (String.IsNullOrEmpty(userId))
                return NotFound();

            var currentUserId = _userManager.GetUserId(User);
            var test = await _testingPlatform.TestManager.FindAsync(t => t.Id == testId && t.AppUserId == currentUserId);

            if (test is null)
                return NotFound();

            var userAnswers = _testingPlatform.UserAnswerManager
                .GetAll()
                .Where(ua => ua.AppUserId == userId && ua.TestAnswer.TestQuestion.TestId == testId);

            var model = new Dictionary<TestQuestion, List<UserAnswer>>();

            foreach(var testQuestion in test.TestQuestions)
            {
                model[testQuestion] = userAnswers.Where(ua => ua.TestAnswer.TestQuestionId == testQuestion.Id).ToList();
            }

            return View(model);
        }
    }
}