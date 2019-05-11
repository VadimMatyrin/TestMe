using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TestMe.Data;
using TestMe.Models;
using TestMe.Sevices;

namespace TestMe.Areas.Identity.Pages.Account
{

    [AllowAnonymous]
    public class RegisterModel : PageModel
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly UserManager<AppUser> _userManager;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;

        public RegisterModel(
            ApplicationDbContext dbContext,
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender)
        {
            _dbContext = dbContext;
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
        }

        [BindProperty]
        public Input InputModel { get; set; }

        public string ReturnUrl { get; set; }

        public enum UserTypes
        {
            Student,
            Teacher
        }

        public class Input
        {
            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [Display(Name = "Name")]
            public string Name { get; set; }

            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Display(Name = "Group")]
            public string Group { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }
            public UserTypes UserType { get; set; } = UserTypes.Student;

        }

        public void OnGet(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");
            var userWithEmail = await _userManager.FindByEmailAsync(InputModel.Email);
            if (!(userWithEmail is null))
            {
                ModelState.AddModelError(string.Empty, "Email is already in use");
            }
            if (ModelState.IsValid)
            {
                var user = new AppUser { UserName = InputModel.Email, Email = InputModel.Email, Name = InputModel.Name };
                var result = await _userManager.CreateAsync(user, InputModel.Password);
                if (result.Succeeded)
                {
                    if (InputModel.UserType == UserTypes.Teacher)
                    {
                        await _userManager.AddToRoleAsync(user, "Teacher");
                    }
                    else
                    {
                        var group = await _dbContext.Groups.FirstOrDefaultAsync(g => g.Name == InputModel.Group);
                        if (group is null)
                        {
                            ModelState.AddModelError(nameof(InputModel.Group), "This group doesn't exist");
                            foreach (var error in result.Errors)
                            {
                                ModelState.AddModelError(string.Empty, error.Description);
                            }
                            return Page();
                        }
                        var groupUser = new GroupUser { Group = group, AppUserId = user.Id };
                        await _dbContext.GroupUsers.AddAsync(groupUser);
                        await _dbContext.SaveChangesAsync();
                    }

                    _logger.LogInformation("User created a new account with password.");

                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { userId = user.Id, code = code },
                        protocol: Request.Scheme);

                    await _emailSender.SendEmailAsync(InputModel.Email, "Confirm your email",
                        $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return LocalRedirect(returnUrl);
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return Page();
        }
    }
}
