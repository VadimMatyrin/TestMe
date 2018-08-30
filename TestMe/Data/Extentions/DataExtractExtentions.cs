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
                .Include(t => t.TestResults);
        }

        public static IQueryable<TestQuestion> ExtractAll(this DbSet<TestQuestion> dbSet)
        {
            return dbSet
                .Include(tq => tq.Test)
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
            .Include(tr => tr.Test);
        }
    }
}
