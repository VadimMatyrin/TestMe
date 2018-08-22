using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TestMe.Data;
using TestMe.Models;

namespace TestMe.Controllers
{
    public class TestEngineController : Controller
    {
        private ApplicationDbContext _context;
        private IEnumerable<TestAnswer> _testAnswers;
        public TestEngineController(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index(string code)
        {
            var test = await GetTestAsync(code);
            if (test is null)
            {
                return RedirectToAction("Index", "Home");
            }
            return View(test);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> StartTest(string code)
        {
            var testAnswers = await GetTestAnswersAsync(code);

            if (testAnswers is null)
                return RedirectToAction("Index", "Home");

            //if (testAnswers.TestQuestions.Any(t => !(t.TestAnswers is null)))
            //    return Json("error");

            _testAnswers = testAnswers;
            return Json(_testAnswers.FirstOrDefault().TestQuestion);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CheckAnswer(int questionId, params int[] answerId)
        {
            throw new NotImplementedException();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GetNextQuestion(int questionId)
        {
            throw new NotImplementedException();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GetPrevQuestion(int questionId)
        {
            throw new NotImplementedException();
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
            if (test is null)
            {
                return null;
            }
            return test;
        }
        private async Task<IEnumerable<TestAnswer>> GetTestAnswersAsync(string code)
        {
            if (code is null)
            {
                return null;
            }
            var testAnswers = _context.TestAnswers.Include(t => t.AppUser).Include(t => t.TestQuestion).ThenInclude(t => t.Test).Where(t => t.TestQuestion.Test.TestCode == code);
            if (testAnswers is null)
            {
                return null;
            } 
            return await testAnswers.ToListAsync();
        }
    }
}