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
        public DbSet<TestQuestion> TestQuestions { get; set; }
        public DbSet<TestAnswer> TestAnswers { get; set; }
        public DbSet<TestResult> TestResults { get; set; }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Test>()
                .Property(t => t.CreationDate)
                .HasDefaultValueSql("getdate()");

            modelBuilder.Entity<Test>().HasMany(t => t.TestQuestions).WithOne(t => t.Test);
            modelBuilder.Entity<TestQuestion>().HasMany(t => t.TestAnswers).WithOne(t => t.TestQuestion);

            //modelBuilder.Entity<Test>()
            //    .HasAlternateKey(t => t.TestCode);

            base.OnModelCreating(modelBuilder);
        }
    }
}
