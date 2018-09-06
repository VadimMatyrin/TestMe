using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TestMe.Sevices.Interfaces;

namespace TestMe.Controllers
{
    public class TestResultsController : Controller
    {
        private readonly ITestingPlatform _testingPlatform;
        public TestResultsController(ITestingPlatform testingPlatform)
        {
            _testingPlatform = testingPlatform;
        }
        public async Task<IActionResult> Index(int? id)
        {
            if (id is null)
                return NotFound();

            var test = await _testingPlatform.TestManager.FindAsync(t => t.Id == id);
            if (test is null)
                return NotFound();

            var questionAmount = _testingPlatform.TestQuestionManager.GetAll().Count(tq => tq.TestId == id);
            ViewBag.questionAmount = questionAmount;
            return View(test);
        }
    }
}