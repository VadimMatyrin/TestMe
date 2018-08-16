using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TestMe.Data;
using TestMe.Models;

namespace TestMe.Controllers
{
    public class TestQuestionsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        public TestQuestionsController(ApplicationDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        // GET: TestQuestions
        public async Task<IActionResult> Index(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var test = await _context.Tests.FindAsync(id);
            if (test == null)
            {
                return NotFound();
            }

            var userName = await _userManager.FindByNameAsync(User.Identity.Name);
            var applicationDbContext = _context.TestQuestion.Include(t => t.Test).Where(t => t.AppUser.Id == userName.Id && t.TestId == id);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: TestQuestions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var testQuestion = await _context.TestQuestion
                .Include(t => t.Test)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (testQuestion == null)
            {
                return NotFound();
            }

            return View(testQuestion);
        }

        // GET: TestQuestions/Create
        public IActionResult Create()
        {
            ViewData["TestId"] = new SelectList(_context.Tests, "Id", "TestName");
            return View();
        }

        // POST: TestQuestions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,QuestionText, TestId")] TestQuestion testQuestion)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByNameAsync(User.Identity.Name);
                testQuestion.AppUser = user;
                _context.Add(testQuestion);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index), new { id = testQuestion.TestId });
            }
            ViewData["TestId"] = new SelectList(_context.Tests, "Id", "TestName", testQuestion.TestId);
            return View("Index", testQuestion.TestId);
        }

        // GET: TestQuestions/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var testQuestion = await _context.TestQuestion.FindAsync(id);
            if (testQuestion == null)
            {
                return NotFound();
            }
            ViewData["TestId"] = new SelectList(_context.Tests, "Id", "TestName", testQuestion.TestId);
            return View(testQuestion);
        }

        // POST: TestQuestions/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,QuestionText,TestId")] TestQuestion testQuestion)
        {
            if (id != testQuestion.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var user = await _userManager.FindByNameAsync(User.Identity.Name);
                    testQuestion.AppUser = user;
                    _context.Update(testQuestion);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TestQuestionExists(testQuestion.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index), new { id = testQuestion.TestId });
            }
            ViewData["TestId"] = new SelectList(_context.Tests, "Id", "TestName", testQuestion.TestId);
            return View(testQuestion);
        }

        // GET: TestQuestions/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var testQuestion = await _context.TestQuestion
                .Include(t => t.Test)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (testQuestion == null)
            {
                return NotFound();
            }

            return View(testQuestion);
        }

        // POST: TestQuestions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var testQuestion = await _context.TestQuestion.FindAsync(id);
            var testId = testQuestion.TestId;
            _context.TestQuestion.Remove(testQuestion);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index), new { id = testId });
        }

        private bool TestQuestionExists(int id)
        {
            return _context.TestQuestion.Any(e => e.Id == id);
        }
    }
}
