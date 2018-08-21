using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TestMe.Data;
using TestMe.Models;

namespace TestMe.Views
{
    public class TestEngineModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        public Test Test { get; set; }
        public TestEngineModel(ApplicationDbContext context)
        {
            _context = context;
        }
        public void OnGet(string code)
        {
            var test = _context.Tests
               .Include(t => t.AppUser)
               .FirstOrDefault(t => t.TestCode == code);

            if (!(test.TestCode is null))
            {
                Test = test;
            }
            else
            {
                Response.Redirect("~/Home/Index");
            }
        }
    }
}