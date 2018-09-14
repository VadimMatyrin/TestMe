using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TestMe.Models;

namespace TestMe.Middleware
{
    public class LogOutBannedUserMiddleware
    {
        private readonly RequestDelegate next;
        public LogOutBannedUserMiddleware(RequestDelegate next)
        { 
            this.next = next;   
        }

        public async Task Invoke(HttpContext context, UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
        {
            if (context.User.Identity.IsAuthenticated)
            {
                var user = await userManager.FindByNameAsync(context.User.Identity.Name);
                if (user.IsBanned)
                {
                    await signInManager.SignOutAsync();
                    context.Response.Redirect("/Identity/Account/Login");
                }
            }
            await next(context);
        }

    }
}
