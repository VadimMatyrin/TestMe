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

            //var testResults = _testingPlatform.TestResultManager.GetAll().Where(t => t.TestId == id).ToList();  //TestManager.FindAsync(t => t.Id == id);
            //var test = testResults.FirstOrDefault()?.Test;
            var test = await _testingPlatform.TestManager.FindAsync(t => t.Id == id);
            if (test is null)
                return NotFound();

            var questionAmount = _testingPlatform.TestQuestionManager.GetAll().Where(tq => tq.TestId == id).Count();
            ViewBag.questionAmount = questionAmount;
            return View(test);
        }
    }
}