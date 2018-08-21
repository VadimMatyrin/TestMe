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
        private Test _test;
        public TestEngineController(ApplicationDbContext context)
        {
            _context = context;
            _test = new Test();
        }
        public async Task<IActionResult> Index(string code)
        {
            if (code == null)
            {
                return RedirectToAction("Index", "Home");
            }
            var test = await _context.Tests.Include(t => t.AppUser).Include(t => t.TestQuestions).FirstOrDefaultAsync(t => t.TestCode == code);
            if (test == null)
            {
                return RedirectToAction("Index", "Home");
            }
            _test = test;
            //_test.TestQuestions = await _context.Entry(_test).Collection(t => t.TestQuestions).Query().ToListAsync();
            return View(test);
        }

        public async Task<IActionResult> StartTest()
        {
            if (_test.TestQuestions.Any(t => !(t.TestAnswers is null)))
                return Json("error");

            foreach (var testQuestion in _test.TestQuestions)
                testQuestion.TestAnswers = await _context.Entry(testQuestion).Collection(t => t.TestAnswers).Query().ToListAsync();
            return Json("hello");
        }
    }
}