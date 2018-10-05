    using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TestMe.Models;

namespace TestMe.Data.Interfaces
{
    public interface ITestingPlatformDbContext : IDisposable
    {
        DbSet<AppUser> AppUsers { get; set; }
        DbSet<Test> Tests { get; set; }
        DbSet<TestQuestion> TestQuestions { get; set; }
        DbSet<TestAnswer> TestAnswers { get; set; }
        DbSet<TestResult> TestResults { get; set; }
        DbSet<TestReport> TestReports { get; set; }
        DbSet<TestMark> TestMarks { get; set; }
        DbSet<UserAnswer> UserAnswers { get; set; }
        Task SaveChangesAsync();
        EntityEntry<T> Entry<T>(T obj) where T : class;
    }
}
