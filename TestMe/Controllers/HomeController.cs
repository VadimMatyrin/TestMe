using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TestMe.Data;
using TestMe.Models;
using TestMe.Sevices.Interfaces;

namespace TestMe.Controllers
{
    public class HomeController : Controller
    {
        readonly ITestingPlatform _testingPlatform;
        readonly UserManager<AppUser> _userManager;
        public HomeController(ITestingPlatform testingPlatform, UserManager<AppUser> userManager)
        {
            _testingPlatform = testingPlatform;
            _userManager = userManager;
        }
        public IActionResult Index()
        {
            var topRatedTest = _testingPlatform.TestManager.GetAll()
                .Where(t => !(t.TestCode == null) && t.TestMarks.Count(tm => tm.EnjoyedTest) - t.TestMarks.Count(tm => !tm.EnjoyedTest) >= 1).Take(10)
                .OrderByDescending(t => t.TestMarks.Count(tm => tm.EnjoyedTest) - t.TestMarks.Count(tm => !tm.EnjoyedTest));
            if (topRatedTest is null)
                return NotFound();

            return View(topRatedTest);
        }
        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
