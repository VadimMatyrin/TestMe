using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        public TestQuestionsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: TestQuestions
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.TestQuestion.Include(t => t.Test);
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
        public async Task<IActionResult> Create([Bind("Id,QuestionText,TestId")] TestQuestion testQuestion)
        {
            if (ModelState.IsValid)
            {
                _context.Add(testQuestion);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["TestId"] = new SelectList(_context.Tests, "Id", "TestName", testQuestion.TestId);
            return View(testQuestion);
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
                return RedirectToAction(nameof(Index));
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
            _context.TestQuestion.Remove(testQuestion);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TestQuestionExists(int id)
        {
            return _context.TestQuestion.Any(e => e.Id == id);
        }
    }
}
