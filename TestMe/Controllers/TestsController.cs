using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TestMe.Data;
using TestMe.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Filters;
using TestMe.Sevices.Interfaces;

namespace TestMe.Controllers
{
    [Authorize]
    public class TestsController : Controller
    {
        private readonly ITestingPlatform _testingPlatform;
        private readonly UserManager<AppUser> _userManager;
        private string _userId;
        public TestsController(ITestingPlatform testingPlatform, UserManager<AppUser> userManager)
        {
            _testingPlatform = testingPlatform;
            _userManager = userManager;
        }
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            _userId = _userManager.GetUserId(User);
        }
        // GET: Tests
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _testingPlatform.TestManager.GetAll().Where(t => t.AppUserId == _userId);

            return View(await applicationDbContext.ToListAsync());
        }
        public async Task<IActionResult> UserResults(int? id)
        {
            if (id == null)
            {
                return RedirectToAction(nameof(Index));
            }

            var test = await GetTestAsync(id);
            if (test == null)
            {
                return RedirectToAction(nameof(Index));
            }
            ViewBag.QuestionAmount = test.TestQuestions.Count();
            return View(test.TestResults);
        }
        public async Task<IActionResult> StopSharing(int? id)
        {
            if (id == null)
            {
                return RedirectToAction(nameof(Index));
            }

            var test = await GetTestAsync(id);
            if (test == null)
            {
                return RedirectToAction(nameof(Index));
            }
            try
            {
                test.TestCode = null;
                await _testingPlatform.TestManager.UpdateAsync(test);
            }
            catch (DbUpdateException)
            {
                if (GetTestAsync(test.Id) is null)
                {
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> CreateCode(int? id)
        {
            if (id == null)
            {
                return RedirectToAction(nameof(Index));
            }

            var test = await GetTestAsync(id);
                
            if (test == null)
            {
                return RedirectToAction(nameof(Index));
            }
            if (test.TestCode is null)
            {
                var generatedCode = _testingPlatform.RandomStringGenerator.RandomString(8);
                try
                {
                    test.TestCode = generatedCode;
                   // test.CreationDate = DateTime.Now;
                    await _testingPlatform.TestManager.UpdateAsync(test);
                }
                catch (DbUpdateException)
                {
                    if (GetTestAsync(test.Id) is null)
                    {
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        throw;
                    }
                }
                return View("CreateCode", generatedCode);
            }
            return View("CreateCode", test.TestCode);
        }
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return RedirectToAction(nameof(Index));
            }

            var test = await GetTestAsync(id);
            if (test == null)
            {
                return RedirectToAction(nameof(Index));
            }

            return View(test);
        }

        public IActionResult Create()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Test test)
        {
            if (_testingPlatform.TestManager.GetAll().Where(t => t.AppUserId == _userId).Any(t => t.TestName == test.TestName))
            {
                ModelState.AddModelError("TestName", "You already have test with the same name!");
            }
            if (ModelState.IsValid)
            {
                test.AppUserId = _userId;
                await _testingPlatform.TestManager.AddAsync(test);
                return RedirectToAction(nameof(Index));
            }
            return View(test);
        }

        // GET: Tests/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return RedirectToAction(nameof(Index));
            }

            var test = await GetTestAsync(id);
            if (test == null)
            {
                return RedirectToAction(nameof(Index));
            }
            return View(test);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,TestName,CreationDate, TestDuration")] Test test)
        {
            if (id != test.Id)
            {
                return RedirectToAction(nameof(Index));
            }

            if (ModelState.IsValid)
            {
                try
                {
                    test.AppUserId = _userId;
                    await _testingPlatform.TestManager.UpdateAsync(test);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (GetTestAsync(test.Id) is null)
                    {
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(test);
        }

        // GET: Tests/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return RedirectToAction(nameof(Index));
            }

            var test = await _testingPlatform.TestManager.FindAsync(m => m.Id == id && m.AppUserId == _userId);
            if (test == null)
            {
                return RedirectToAction(nameof(Index));
            }

            return View(test);
        }

        // POST: Tests/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var test = await _testingPlatform.TestManager.FindAsync(m => m.Id == id && m.AppUserId == _userId);
            await _testingPlatform.TestManager.DeleteAsync(test);
            return RedirectToAction(nameof(Index));
        }

        private async Task<Test> GetTestAsync(int? id)
        {
            if (id is null)
                throw new ArgumentNullException();

            return await _testingPlatform.TestManager.FindAsync(m => m.Id == id && m.AppUserId == _userId);
        }
    }
}
