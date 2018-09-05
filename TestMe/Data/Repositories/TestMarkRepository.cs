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
    public class TestMarkRepository:IRepository<TestMark>
    {
        private readonly ITestingPlatformDbContext _db;
        public TestMarkRepository(ITestingPlatformDbContext db)
        {
            _db = db;
        }
        public async Task AddAsync(TestMark testResult)
        {
            if (testResult is null)
                throw new ArgumentNullException(nameof(testResult));

            await _db.TestMarks.AddAsync(testResult);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteAsync(TestMark testResult)
        {
            if (testResult is null)
                throw new ArgumentNullException(nameof(testResult));

            _db.TestMarks.Remove(testResult);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteRangeAsync(IEnumerable<TestMark> testResult)
        {
            if (testResult is null)
                throw new ArgumentNullException(nameof(testResult));

            _db.TestMarks.RemoveRange(testResult);
            await _db.SaveChangesAsync();
        }

        public async Task<TestMark> FindAsync(Predicate<TestMark> predicate)
        {
            return await _db.TestMarks.ExtractAll().FirstOrDefaultAsync(tr => predicate(tr));
        }

        public IQueryable<TestMark> GetAll()
        {
            return _db.TestMarks.ExtractAll();
        }

        public async Task UpdateAsync(TestMark testResult)
        {
            if (testResult is null)
                throw new ArgumentNullException(nameof(testResult));

            _db.Entry(testResult).State = EntityState.Modified;
            await _db.SaveChangesAsync();
        }
    }
}
