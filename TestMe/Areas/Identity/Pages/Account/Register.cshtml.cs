using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
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
    public enum UserTypes
    {
        Student,
        Teacher
    }
    public class RegisterModel
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

    public class LoginModel
    {
        //[Required]
        public string Username { get; set; }

        //[Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }


    public class Model : PageModel
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly UserManager<AppUser> _userManager;
        private readonly ILogger<Model> _logger;
        private readonly IEmailSender _emailSender;

        [TempData]
        public string ErrorMessage { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public Model(
           ApplicationDbContext dbContext,
           UserManager<AppUser> userManager,
           SignInManager<AppUser> signInManager,
           ILogger<Model> logger,
           IEmailSender emailSender)
        {
            _dbContext = dbContext;
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
        }

        [BindProperty]
        public RegisterModel RegisterModel { get; set; }

        [BindProperty]
        public LoginModel LoginModel { get; set; }

        public string ReturnUrl { get; set; }

        public async Task OnGetAsync(string returnUrl = null)
        {
            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                ModelState.AddModelError(string.Empty, ErrorMessage);
            }

            returnUrl = returnUrl ?? Url.Content("~/");

            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            ReturnUrl = returnUrl;
        }

        public Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            if (RegisterModel.Email is null || RegisterModel.Password is null)
                return LoginUser(returnUrl);

            return RegisterUser(returnUrl);

        }

        public async Task<IActionResult> RegisterUser(string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");
            var userWithEmail = await _userManager.FindByEmailAsync(RegisterModel.Email);
            if (!(userWithEmail is null))
            {
                ModelState.AddModelError(string.Empty, "Email is already in use");
            }
            if (ModelState.IsValid)
            {
                var user = new AppUser { UserName = RegisterModel.Email, Email = RegisterModel.Email, Name = RegisterModel.Name };
                var result = await _userManager.CreateAsync(user, RegisterModel.Password);
                if (result.Succeeded)
                {
                    if (RegisterModel.UserType == UserTypes.Teacher)
                    {
                        await _userManager.AddToRoleAsync(user, "Teacher");
                    }
                    else
                    {
                        var group = await _dbContext.Groups.FirstOrDefaultAsync(g => g.Name == RegisterModel.Group);
                        if (group is null)
                        {
                            ModelState.AddModelError(nameof(RegisterModel.Group), "This group doesn't exist");
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

                    await _emailSender.SendEmailAsync(RegisterModel.Email, "Confirm your email",
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

        public async Task<IActionResult> LoginUser(string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");
            ModelState.Clear();
            if(LoginModel.Username.Length == 0)
            {
                ModelState.AddModelError("Username", "Username is empty");
            }
            if (LoginModel.Password.Length == 0)
            {
                ModelState.AddModelError("Password", "Password is empty");
            }
            if (ModelState.IsValid)
            {
                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, set lockoutOnFailure: true
                var user = await _userManager.FindByNameAsync(LoginModel.Username);
                if (user is null)
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attampt");
                    return Page();
                }
                if (user.IsBanned && (await _signInManager.CheckPasswordSignInAsync(user, LoginModel.Password, false)).Succeeded)
                {
                    await _signInManager.SignOutAsync();
                    ModelState.AddModelError(string.Empty, "Your account was banned");
                    return Page();
                }
                var result = await _signInManager.PasswordSignInAsync(LoginModel.Username, LoginModel.Password, LoginModel.RememberMe, lockoutOnFailure: true);
                if (result.Succeeded)
                {
                    _logger.LogInformation("User logged in.");
                    return LocalRedirect(returnUrl);
                }
                if (result.RequiresTwoFactor)
                {
                    return RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl, RememberMe = LoginModel.RememberMe });
                }
                if (result.IsLockedOut)
                {
                    _logger.LogWarning("User account locked out.");
                    return RedirectToPage("./Lockout");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    return Page();
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}
