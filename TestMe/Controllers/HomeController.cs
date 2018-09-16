using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using TestMe.Config;
using TestMe.Data;
using TestMe.Models;
using TestMe.Sevices.Interfaces;

namespace TestMe.Controllers
{
    public class HomeController : Controller
    {
        private readonly ITestingPlatform _testingPlatform;
        private readonly UserManager<AppUser> _userManager;
        private readonly IOptions<LoadConfig> _loadConfig;
        public HomeController(ITestingPlatform testingPlatform, UserManager<AppUser> userManager, IOptions<LoadConfig> loadConfig)
        {
            _testingPlatform = testingPlatform;
            _userManager = userManager;
            _loadConfig = loadConfig;
        }
        public async Task<IActionResult> Index()
        {
            var topRatedTest = await _testingPlatform.TestManager
                .GetAll()
                .Where(t => !(t.TestCode == null) && t.TestMarks.Count(tm => tm.EnjoyedTest) - t.TestMarks.Count(tm => !tm.EnjoyedTest) >= 1)
                .Take(_loadConfig.Value.TopRatedHomePageAmount)
                .OrderByDescending(t => t.TestMarks.Count(tm => tm.EnjoyedTest) - t.TestMarks.Count(tm => !tm.EnjoyedTest))
                .ToListAsync();

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
