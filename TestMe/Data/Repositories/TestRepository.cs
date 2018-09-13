using Microsoft.AspNetCore.Identity;
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
    public class TestRepository : IRepository<Test>
    {
        private readonly ITestingPlatformDbContext _db;
        public TestRepository(ITestingPlatformDbContext db)
        {
            _db = db;
        }

        public async Task AddAsync(Test test)
        {
            if (test is null)
                throw new ArgumentNullException(nameof(test));

            await _db.Tests.AddAsync(test);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteAsync(Test test)
        {
            if (test is null)
                throw new ArgumentNullException(nameof(test));

            _db.Tests.Remove(test);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteRangeAsync(IEnumerable<Test> tests)
        {
            if (tests is null)
                throw new ArgumentNullException(nameof(tests));

            _db.Tests.RemoveRange(tests);
            await _db.SaveChangesAsync();
        }

        public async Task<Test> FindAsync(Predicate<Test> predicate)
        {
            var test = await _db.Tests.ExtractAll().FirstOrDefaultAsync(t => predicate(t));
            test.TestAnswers = await _db.TestAnswers.ExtractAll().Where(ta => ta.TestQuestion.TestId == test.Id).ToListAsync();
            foreach(var testQuestion in test.TestQuestions.Where(tq => tq.TestAnswers == null))
            {
                testQuestion.TestAnswers = new List<TestAnswer>(); 
            }
            return test; 
        }

        public IQueryable<Test> GetAll()
        {
            return _db.Tests.ExtractAll(); ;
        }

        public async Task UpdateAsync(Test test)
        {
            if (test is null)
                throw new ArgumentNullException(nameof(test));

            _db.Entry(test).State = EntityState.Modified;
            _db.Entry<Test>(test).Property(x => x.CreationDate).IsModified = false;
            await _db.SaveChangesAsync();
        }
    }
}
