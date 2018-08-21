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

            var test = await _context.Tests.Include(t => t.AppUser).FirstOrDefaultAsync(t => t.TestCode == code);
            if (test == null)
            {
                return RedirectToAction("Index", "Home");
            }
            _test = test;
            return View(test);
        }
    }
}