using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TestMe.Data;
using TestMe.Data.Extentions;
using TestMe.Models;
using TestMe.Sevices.Interfaces;

namespace TestMe.Controllers
{
    [Authorize]
    public class TestAnswersController : Controller
    {
        private readonly ITestingPlatform _testingPlatform;
        private readonly IHostingEnvironment _appEnvironment;
        private readonly UserManager<AppUser> _userManager;
        private string _userId;
        public TestAnswersController(ITestingPlatform testingPlatform, IHostingEnvironment appEnvironment, UserManager<AppUser> userManager)
        {
            _testingPlatform = testingPlatform;
            _appEnvironment = appEnvironment;
            _userManager = userManager;
        }
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (User.IsInRole("Admin"))
            {
                if (!(context.RouteData.Values["id"] is null))
                {
                    if (Int32.TryParse(context.RouteData.Values["id"].ToString(), out int answerId))
                    {
                        if (context.RouteData.Values["action"].ToString() == "Index" || context.RouteData.Values["action"].ToString() == "Create")
                            _userId = _testingPlatform.TestQuestionManager.GetAll().AsNoTracking().FirstOrDefault(tq => tq.Id == answerId)?.AppUserId;
                        else
                            _userId = _testingPlatform.TestAnswerManager.GetAll().AsNoTracking().FirstOrDefault(t => t.Id == answerId)?.AppUserId;
                    }
                        
                }
            }
            _userId = _userId ?? _userManager.GetUserId(User);
        }
        public async Task<IActionResult> Index(int? id)
        {
            if (id == null)
                return NotFound();

            var testQuestion = await _testingPlatform.TestQuestionManager
                .FindAsync(tq => tq.AppUserId == _userId && tq.Id == id);
            if (testQuestion is null)
                return NotFound();

            return View(testQuestion);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var testAnswer = await _testingPlatform.TestAnswerManager.FindAsync(ta => ta.AppUserId == _userId && ta.Id == id);
            if (testAnswer == null)
                return NotFound();

            return View(testAnswer);
        }

        public async Task<IActionResult> Create(int? id)
        {
            if (id == null)
                return NotFound();

            var testQuestion = await _testingPlatform.TestQuestionManager
                .FindAsync(tq => tq.AppUserId == _userId && tq.Id == id);

            if (testQuestion == null)
                return NotFound();

            if (!(testQuestion.Test.TestCode is null))
                return NotFound();

            var testAnswer = new TestAnswer { TestQuestion = testQuestion, TestQuestionId = testQuestion.Id };
            return View(testAnswer);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("AnswerText,IsCorrect,TestQuestionId")] TestAnswer testAnswer)
        {
            var files = HttpContext.Request.Form.Files;
            if (files.Count != 0 && !files.First().IsImage())
            {
                ModelState.AddModelError("ImageName", "File is not an image");
            }
            if (ModelState.IsValid)
            {

                if (files.Count != 0)
                {
                    var imageName = $"{Guid.NewGuid().ToString().Replace("-", "")}{Path.GetExtension(files.First().FileName)}";
                    if (await _testingPlatform.AnswerImageManager.SaveAnswerImageAsync(files.First(), imageName))
                        testAnswer.ImageName = imageName;
                }


                testAnswer.AppUserId = _userId;
                await _testingPlatform.TestAnswerManager.AddAsync(testAnswer);
                return RedirectToAction(nameof(Index), new { id = testAnswer.TestQuestionId });
            }
            else
            {
                var testQuestion = await _testingPlatform.TestQuestionManager
                    .FindAsync(tq => tq.AppUserId == _userId && tq.Id == testAnswer.TestQuestionId);
                if (testQuestion == null)
                {
                    return NotFound();
                }
                testAnswer.TestQuestion = testQuestion;
            }
            return View(testAnswer);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var testAnswer = await _testingPlatform.TestAnswerManager.FindAsync(ta => ta.AppUserId == _userId && ta.Id == id);
            if (testAnswer == null)
            {
                return NotFound();
            }
            if (!(testAnswer.TestQuestion.Test.TestCode is null))
                return NotFound();

            return View(testAnswer);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id, AnswerText,IsCorrect,TestQuestionId, ImageName")] TestAnswer testAnswer)
        {
            if (id != testAnswer.Id)
            {
                return NotFound();
            }
            var files = HttpContext.Request.Form.Files;
            if (files.Count != 0 && !files.First().IsImage())
            {
                ModelState.AddModelError("ImageName", "File is not an image");
            }
            if (ModelState.IsValid)
            {
                if (files.Count != 0)
                {
                    if(testAnswer.ImageName is null || _testingPlatform.AnswerImageManager.DeleteAnswerImage(testAnswer.ImageName))
                    {
                        var imageName = $"{Guid.NewGuid().ToString().Replace("-", "")}{Path.GetExtension(files.First().FileName)}";
                        if(await _testingPlatform.AnswerImageManager.SaveAnswerImageAsync(files.First(), imageName))
                             testAnswer.ImageName = imageName;
                    }
                }
                else
                {
                    testAnswer.ImageName = (await _testingPlatform.TestAnswerManager
                        .GetAll()
                        .AsNoTracking()
                        .FirstOrDefaultAsync(ta => ta.Id == id))
                        .ImageName;
                }
                testAnswer.AppUserId = _userId;
                try
                {
                    await _testingPlatform.TestAnswerManager.UpdateAsync(testAnswer);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (await _testingPlatform.TestAnswerManager.FindAsync(ta => ta.AppUserId == _userId && ta.Id == id) is null)
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index), new { id = testAnswer.TestQuestionId });
            }

            testAnswer = await _testingPlatform.TestAnswerManager.FindAsync(ta => ta.AppUserId == _userId && ta.Id == id);
            return View(testAnswer);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var testAnswer = await _testingPlatform.TestAnswerManager.FindAsync(ta => ta.AppUserId == _userId && ta.Id == id);
            if (testAnswer == null)
                return NotFound();

            if (!(testAnswer.TestQuestion.Test.TestCode is null))
                return RedirectToAction("Index", new { id = testAnswer.TestQuestionId });

            return View(testAnswer);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var testAnswer = await _testingPlatform.TestAnswerManager.FindAsync(ta => ta.AppUserId == _userId && ta.Id == id);
            if (!(testAnswer.ImageName is null))
                _testingPlatform.AnswerImageManager.DeleteAnswerImage(testAnswer.ImageName);

            await _testingPlatform.TestAnswerManager.DeleteAsync(testAnswer);
            return RedirectToAction(nameof(Index), new { id = testAnswer.TestQuestionId });
        }

        public async Task<IActionResult> DeletePhoto(int? id)
        {
            if (id is null)
                return NotFound();

            var testAnswer = await _testingPlatform.TestAnswerManager.FindAsync(ta => ta.AppUserId == _userId && ta.Id == id);
            if(testAnswer is null)
                return NotFound();

            if (!(testAnswer.ImageName is null))
                if (_testingPlatform.AnswerImageManager.DeleteAnswerImage(testAnswer.ImageName))
                {
                    testAnswer.ImageName = null;
                    await _testingPlatform.TestAnswerManager.UpdateAsync(testAnswer);
                }

            return RedirectToAction("Index", new { id = testAnswer.TestQuestionId });
        }

    }
}
