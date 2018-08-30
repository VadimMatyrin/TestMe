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
    public class TestAnswerRepository : IRepository<TestAnswer>
    {
        private readonly ITestingPlatformDbContext _db;
        public TestAnswerRepository(ITestingPlatformDbContext db)
        {
            _db = db;
        }
        public async Task AddAsync(TestAnswer testAnswer)
        {
            if (testAnswer is null)
                throw new ArgumentNullException(nameof(testAnswer));

            await _db.TestAnswers.AddAsync(testAnswer);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteAsync(TestAnswer testAnswer)
        {
            if (testAnswer is null)
                throw new ArgumentNullException(nameof(testAnswer));

            _db.TestAnswers.Remove(testAnswer);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteRangeAsync(IEnumerable<TestAnswer> testAnswer)
        {
            if (testAnswer is null)
                throw new ArgumentNullException(nameof(testAnswer));

            _db.TestAnswers.RemoveRange(testAnswer);
            await _db.SaveChangesAsync();
        }

        public async Task<TestAnswer> FindAsync(Predicate<TestAnswer> predicate)
        {
            return await _db.TestAnswers.ExtractAll().FirstOrDefaultAsync(ta => predicate(ta));
        }

        public IQueryable<TestAnswer> GetAll()
        {
            return _db.TestAnswers.ExtractAll();
        }

        public async Task UpdateAsync(TestAnswer testAnswer)
        {
            if (testAnswer is null)
                throw new ArgumentNullException(nameof(testAnswer));

            _db.Entry(testAnswer).State = EntityState.Modified;
            await _db.SaveChangesAsync();
        }
    }
}
