using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
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
        private string _userId;
        public TestQuestionsController(ApplicationDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            //Get user id   
            _userId = _userManager.GetUserId(User);
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

            //var user = await _userManager.GetUserAsync(User);
            var applicationDbContext = _context.TestQuestions.Include(t => t.Test).Where(t => t.AppUser.Id == _userId && t.TestId == id);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: TestQuestions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var testQuestion = await _context.TestQuestions
                .Include(t => t.Test)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (testQuestion == null)
            {
                return NotFound();
            }

            return View(testQuestion);
        }

        // GET: TestQuestions/Create
        public async Task<IActionResult> Create(int? id)
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
            //var user = await _userManager.GetUserAsync(User);
            ViewBag.TestId = test.Id;
            ViewData["Tests"] = new SelectList(_context.Tests.Where(t => t.AppUserId == _userId), "Id", "TestName");
            return View();
        }

        // POST: TestQuestions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("QuestionText", "TestId")] TestQuestion testQuestion)
        {
            if (ModelState.IsValid)
            {
                //var user = await _userManager.FindByNameAsync(User.Identity.Name);
                //testQuestion.AppUser = user;
                testQuestion.AppUserId = _userId;
                _context.Add(testQuestion);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index), new { id = testQuestion.TestId });
            }
            ViewData["Tests"] = new SelectList(_context.Tests, "Id", "TestName", testQuestion.TestId);
            return View();
        }

        // GET: TestQuestions/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var testQuestion = await _context.TestQuestions.FindAsync(id);
            if (testQuestion == null)
            {
                return NotFound();
            }
            ViewData["Tests"] = new SelectList(_context.Tests, "Id", "TestName", testQuestion.TestId);
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
                    //var user = await _userManager.FindByNameAsync(User.Identity.Name);
                    //testQuestion.AppUser = user;
                    testQuestion.AppUserId = _userId;
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
            ViewData["Tests"] = new SelectList(_context.Tests, "Id", "TestName", testQuestion.TestId);
            return View(testQuestion);
        }

        // GET: TestQuestions/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var testQuestion = await _context.TestQuestions
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
            var testQuestion = await _context.TestQuestions.FindAsync(id);
            var testId = testQuestion.TestId;
            _context.TestQuestions.Remove(testQuestion);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index), new { id = testId });
        }

        private bool TestQuestionExists(int id)
        {
            return _context.TestQuestions.Any(e => e.Id == id);
        }
    }
}
