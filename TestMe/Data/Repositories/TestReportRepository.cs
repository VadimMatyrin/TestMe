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
    public class TestReportRepository : IRepository<TestReport>
    {
        private readonly ITestingPlatformDbContext _db;
        public TestReportRepository(ITestingPlatformDbContext db)
        {
            _db = db;
        }

        public async Task AddAsync(TestReport testReport)
        {
            if (testReport is null)
                throw new ArgumentNullException(nameof(testReport));

            await _db.TestReports.AddAsync(testReport);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteAsync(TestReport testReport)
        {
            if (testReport is null)
                throw new ArgumentNullException(nameof(testReport));

            _db.TestReports.Remove(testReport);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteRangeAsync(IEnumerable<TestReport> testReports)
        {
            if (testReports is null)
                throw new ArgumentNullException(nameof(testReports));

            _db.TestReports.RemoveRange(testReports);
            await _db.SaveChangesAsync();
        }

        public async Task<TestReport> FindAsync(Predicate<TestReport> predicate)
        {
            return await _db.TestReports.ExtractAll().FirstOrDefaultAsync(test => predicate(test));
        }

        public IQueryable<TestReport> GetAll()
        {
            return _db.TestReports.ExtractAll();
        }

        public async Task UpdateAsync(TestReport test)
        {
            if (test is null)
                throw new ArgumentNullException(nameof(test));

            _db.Entry(test).State = EntityState.Modified;
            await _db.SaveChangesAsync();
        }
    }
}
