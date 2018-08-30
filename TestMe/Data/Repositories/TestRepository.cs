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
            return await _db.Tests.ExtractAll().FirstOrDefaultAsync(test => predicate(test));
        }

        public IQueryable<Test> GetAll()
        {
            return _db.Tests.ExtractAll();
        }

        public async Task UpdateAsync(Test test)
        {
            if (test is null)
                throw new ArgumentNullException(nameof(test));

            _db.Entry(test).State = EntityState.Modified;
            await _db.SaveChangesAsync();
        }
    }
}
