using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TestMe.Models;

namespace TestMe.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public DbSet<AppUser> AppUsers { get; set; }
        public DbSet<Test> Tests { get; set; }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
    }
}
