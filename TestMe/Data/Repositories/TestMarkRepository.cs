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
        public async Task AddAsync(TestMark testMark)
        {
            if (testMark is null)
                throw new ArgumentNullException(nameof(testMark));

            await _db.TestMarks.AddAsync(testMark);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteAsync(TestMark testMark)
        {
            if (testMark is null)
                throw new ArgumentNullException(nameof(testMark));

            _db.TestMarks.Remove(testMark);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteRangeAsync(IEnumerable<TestMark> testMark)
        {
            if (testMark is null)
                throw new ArgumentNullException(nameof(testMark));

            _db.TestMarks.RemoveRange(testMark);
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

        public async Task UpdateAsync(TestMark testMark)
        {
            if (testMark is null)
                throw new ArgumentNullException(nameof(testMark));

            _db.Entry(testMark).State = EntityState.Modified;
            await _db.SaveChangesAsync();
        }
    }
}
