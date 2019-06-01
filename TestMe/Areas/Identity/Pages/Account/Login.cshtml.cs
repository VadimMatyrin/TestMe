using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using TestMe.Models;

namespace TestMe.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class LoginModelOld : PageModel
    {
        public async Task<IActionResult> OnGetAsync(string returnUrl = null)
        {
            return RedirectToAction("Register", "Identity/Account");
        }
    }
}
