using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TestMe.Models;

namespace TestMe.Data.Extentions
{
    public static class DataExtractExtentions
    {
        public static IQueryable<Test> ExtractAll(this DbSet<Test> dbSet)
        {
            return dbSet
                .Include(t => t.AppUser)
                .Include(t => t.TestQuestions)
                .Include(t => t.TestAnswers)
                .Include(t => t.TestResults)
                .Include(t => t.TestReports)
                .Include(t => t.TestMarks);
        }

        public static IQueryable<TestQuestion> ExtractAll(this DbSet<TestQuestion> dbSet)
        {
            return dbSet
                .Include(tq => tq.Test)
                .ThenInclude(t => t.TestResults)
                .Include(t => t.Test)
                .ThenInclude(t => t.TestReports)
                .Include(tq => tq.TestAnswers)
                .Include(tq => tq.AppUser);
        }

        public static IQueryable<TestAnswer> ExtractAll(this DbSet<TestAnswer> dbSet)
        {
            return dbSet
            .Include(ta => ta.AppUser)
            .Include(ta => ta.TestQuestion)
            .ThenInclude(tq => tq.Test);
        }
        public static IQueryable<TestResult> ExtractAll(this DbSet<TestResult> dbSet)
        {
            return dbSet
            .Include(tr => tr.Test)
            .ThenInclude(t => t.AppUser)
            .Include(tr => tr.AppUser);
        }
        public static IQueryable<TestReport> ExtractAll(this DbSet<TestReport> dbSet)
        {
            return dbSet
            .Include(tr => tr.Test)
            .ThenInclude(t => t.AppUser)
            .Include(tr => tr.AppUser);
        }
        public static IQueryable<TestMark> ExtractAll(this DbSet<TestMark> dbSet)
        {
            return dbSet
            .Include(tr => tr.Test)
            .ThenInclude(t => t.AppUser)
            .Include(tr => tr.AppUser);
        }
    }
}
