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
    public class TestAnswersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        public TestAnswersController(ApplicationDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: TestAnswers
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.TestAnswers.Include(t => t.AppUser).Include(t => t.TestQuestion);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: TestAnswers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var testAnswer = await _context.TestAnswers
                .Include(t => t.AppUser)
                .Include(t => t.TestQuestion)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (testAnswer == null)
            {
                return NotFound();
            }

            return View(testAnswer);
        }

        // GET: TestAnswers/Create
        public IActionResult Create()
        {
            ViewData["AppUserId"] = new SelectList(_context.AppUsers, "Id", "Id");
            ViewData["TestQuestionId"] = new SelectList(_context.TestQuestions, "Id", "QuestionText");
            return View();
        }

        // POST: TestAnswers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,AnswerText,IsCorrect,TestQuestionId,AppUserId")] TestAnswer testAnswer)
        {
            if (ModelState.IsValid)
            {
                _context.Add(testAnswer);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["AppUserId"] = new SelectList(_context.AppUsers, "Id", "Id", testAnswer.AppUserId);
            ViewData["TestQuestionId"] = new SelectList(_context.TestQuestions, "Id", "QuestionText", testAnswer.TestQuestionId);
            return View(testAnswer);
        }

        // GET: TestAnswers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var testAnswer = await _context.TestAnswers.FindAsync(id);
            if (testAnswer == null)
            {
                return NotFound();
            }
            ViewData["AppUserId"] = new SelectList(_context.AppUsers, "Id", "Id", testAnswer.AppUserId);
            ViewData["TestQuestionId"] = new SelectList(_context.TestQuestions, "Id", "QuestionText", testAnswer.TestQuestionId);
            return View(testAnswer);
        }

        // POST: TestAnswers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,AnswerText,IsCorrect,TestQuestionId,AppUserId")] TestAnswer testAnswer)
        {
            if (id != testAnswer.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(testAnswer);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TestAnswerExists(testAnswer.Id))
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
            ViewData["AppUserId"] = new SelectList(_context.AppUsers, "Id", "Id", testAnswer.AppUserId);
            ViewData["TestQuestionId"] = new SelectList(_context.TestQuestions, "Id", "QuestionText", testAnswer.TestQuestionId);
            return View(testAnswer);
        }

        // GET: TestAnswers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var testAnswer = await _context.TestAnswers
                .Include(t => t.AppUser)
                .Include(t => t.TestQuestion)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (testAnswer == null)
            {
                return NotFound();
            }

            return View(testAnswer);
        }

        // POST: TestAnswers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var testAnswer = await _context.TestAnswers.FindAsync(id);
            _context.TestAnswers.Remove(testAnswer);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TestAnswerExists(int id)
        {
            return _context.TestAnswers.Any(e => e.Id == id);
        }
    }
}
