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
            _userId = _userManager.GetUserId(User);
        }
        public async Task<IActionResult> Index(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var testQuestion = await _testingPlatform.TestQuestionManager.GetAll().FirstAsync(tq => tq.AppUserId == _userId && tq.Id == id);
            var test = testQuestion.Test;
            if (test == null)
            {
                return NotFound();
            }
            return View(test);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var testAnswer = await _testingPlatform.TestAnswerManager.GetTestAnswerAsync(_userId, id);
            if (testAnswer == null)
            {
                return NotFound();
            }

            return View(testAnswer);
        }

        public async Task<IActionResult> Create(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var testQuestion = await _testingPlatform.TestQuestionManager.GetAll().FirstAsync(tq => tq.AppUserId == _userId && tq.Id == id);
            if (testQuestion == null)
            {
                return NotFound();
            }
            if (!(testQuestion.Test.TestCode is null))
                return RedirectToAction("Index", new { id });

            var testAnswer = new TestAnswer { TestQuestion = testQuestion };
            return View(testAnswer);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("AnswerText,IsCorrect,TestQuestionId")] TestAnswer testAnswer)
        {
            if (ModelState.IsValid)
            {

                var files = HttpContext.Request.Form.Files;
                foreach (var Image in files)
                {
                    if (Image != null && Image.Length > 0)
                    {
                        var file = Image;
                        var uploads = Path.Combine(_appEnvironment.WebRootPath, "uploads\\answerPics");
                        if (file.Length > 0)
                        {
                            var fileName = $"{Guid.NewGuid().ToString().Replace("-", "")}{Path.GetExtension(file.FileName)}";
                            using (var fileStream = new FileStream(Path.Combine(uploads, fileName), FileMode.Create))
                            {
                                await file.CopyToAsync(fileStream);
                                testAnswer.ImageName = fileName;
                            }

                        }
                    }
                }


                testAnswer.AppUserId = _userId;
                await _testingPlatform.TestAnswerManager.AddAsync(testAnswer);
                return RedirectToAction(nameof(Index), new { id = testAnswer.TestQuestionId });
            }
            return View(testAnswer);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var testAnswer = await _testingPlatform.TestAnswerManager.GetTestAnswerAsync(_userId, id);
            if (testAnswer == null)
            {
                return NotFound();
            }
            if (!(testAnswer.TestQuestion.Test.TestCode is null))
                return RedirectToAction("Index", new { id });

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

            if (ModelState.IsValid)
            {

                var files = HttpContext.Request.Form.Files;
                if (files.Count != 0)
                {
                    var image = files.FirstOrDefault();
                    if (image.IsImage() && testAnswer.ImageName is null ? true : DeleteAnswerImage(testAnswer.ImageName))
                    {
                        testAnswer.ImageName = null;
                        var file = image;
                        var uploads = Path.Combine(_appEnvironment.WebRootPath, "uploads\\answerPics");
                        var fileName = $"{Guid.NewGuid().ToString().Replace("-", "")}{Path.GetExtension(file.FileName)}";
                        using (var fileStream = new FileStream(Path.Combine(uploads, fileName), FileMode.Create))
                        {
                            await file.CopyToAsync(fileStream);
                            testAnswer.ImageName = fileName;
                        }
                    }
                }
                testAnswer.AppUserId = _userId;

                try
                {
                    await _testingPlatform.TestAnswerManager.UpdateAsync(testAnswer);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (await _testingPlatform.TestAnswerManager.GetTestAnswerAsync(_userId, id) is null)
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
            return View(testAnswer);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var testAnswer = await _testingPlatform.TestAnswerManager.GetTestAnswerAsync(_userId, id);
            if (testAnswer == null)
            {
                return NotFound();
            }

            return View(testAnswer);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var testAnswer = await _testingPlatform.TestAnswerManager.GetTestAnswerAsync(_userId, id);
            await _testingPlatform.TestAnswerManager.DeleteAsync(testAnswer);
            return RedirectToAction(nameof(Index), new { id = testAnswer.TestQuestionId });
        }

        private bool DeleteAnswerImage(string imageName)
        {
            var filePath = Path.Combine(_appEnvironment.WebRootPath, $"uploads\\answerPics\\{imageName}");
            var file = new FileInfo(filePath);
            if (file.Exists)
            {
                file.Delete();
                return true;
            }
            return false;
        }

    }
}
