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
    public class TestQuestionRepository : IRepository<TestQuestion>
    {
        private readonly ITestingPlatformDbContext _db;
        public async Task AddAsync(TestQuestion testQuestion)
        {
            if (testQuestion is null)
                throw new ArgumentNullException(nameof(testQuestion));

            await _db.TestQuestions.AddAsync(testQuestion);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteAsync(TestQuestion testQuestion)
        {
            if (testQuestion is null)
                throw new ArgumentNullException(nameof(testQuestion));

            _db.TestQuestions.Remove(testQuestion);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteRangeAsync(IEnumerable<TestQuestion> testQuestions)
        {
            if (testQuestions is null)
                throw new ArgumentNullException(nameof(testQuestions));

            _db.TestQuestions.RemoveRange(testQuestions);
            await _db.SaveChangesAsync();
        }

        public async Task<TestQuestion> FindAsync(Predicate<TestQuestion> predicate)
        {
            return await _db.TestQuestions.ExtractAll().FirstOrDefaultAsync(tq => predicate(tq));
        }

        public IQueryable<TestQuestion> GetAll()
        {
            return _db.TestQuestions.ExtractAll();
        }

        public async Task UpdateAsync(TestQuestion testQuestion)
        {
            if (testQuestion is null)
                throw new ArgumentNullException(nameof(testQuestion));

            _db.Entry(testQuestion).State = EntityState.Modified;
            await _db.SaveChangesAsync();
        }
    }
}
