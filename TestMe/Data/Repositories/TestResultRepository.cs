using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TestMe.Data.Extentions;
using TestMe.Data.Interfaces;
using TestMe.Models;

namespace TestMe.Data.Repositories
{
    public class TestResultRepository : IRepository<TestResult>
    {
        private readonly ITestingPlatformDbContext _db;
        public TestResultRepository(ITestingPlatformDbContext db)
        {
            _db = db;
        }
        public async Task AddAsync(TestResult testResult)
        {
            if (testResult is null)
                throw new ArgumentNullException(nameof(testResult));

            await _db.TestResults.AddAsync(testResult);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteAsync(TestResult testResult)
        {
            if (testResult is null)
                throw new ArgumentNullException(nameof(testResult));

            _db.TestResults.Remove(testResult);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteRangeAsync(IEnumerable<TestResult> testResult)
        {
            if (testResult is null)
                throw new ArgumentNullException(nameof(testResult));

            _db.TestResults.RemoveRange(testResult);
            await _db.SaveChangesAsync();
        }

        public async Task<TestResult> FindAsync(Predicate<TestResult> predicate)
        {
            return await _db.TestResults.ExtractAll().FirstOrDefaultAsync(tr => predicate(tr));
        }

        public IQueryable<TestResult> GetAll()
        {
            return _db.TestResults.ExtractAll();
        }

        public async Task UpdateAsync(TestResult testResult)
        {
            if (testResult is null)
                throw new ArgumentNullException(nameof(testResult));

            _db.Entry(testResult).State = EntityState.Modified;
            await _db.SaveChangesAsync();
        }
    }
}
