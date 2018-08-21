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
        private ICollection<TestQuestion> _testQuestions;
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
            var test = await GetTestAsync(code);

            if (test is null)
                return RedirectToAction("Index", "Home");

            if (test.TestQuestions.Any(t => !(t.TestAnswers is null)))
                return Json("error");

            _testQuestions = test.TestQuestions;

            return Json(_testQuestions.FirstOrDefault());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Tast<IActionResult> CheckAnswer(int questionId, params int[] answerId)
        {

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GetNextQuestion(int questionId)
        {

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GetPrevQuestion(int questionId)
        {

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GetQuestion(int questionId)
        {

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
    }
}