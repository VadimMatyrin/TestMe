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
    public class UserAnswerRepository : IRepository<UserAnswer>
    {
        private readonly ITestingPlatformDbContext _db;
        public UserAnswerRepository(ITestingPlatformDbContext db)
        {
            _db = db;
        }
        public async Task AddAsync(UserAnswer userAnswer)
        {
            if (userAnswer is null)
                throw new ArgumentNullException(nameof(userAnswer));

            await _db.UserAnswers.AddAsync(userAnswer);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteAsync(UserAnswer userAnswer)
        {
            if (userAnswer is null)
                throw new ArgumentNullException(nameof(userAnswer));

            _db.UserAnswers.Remove(userAnswer);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteRangeAsync(IEnumerable<UserAnswer> userAnswers)
        {
            if (userAnswers is null)
                throw new ArgumentNullException(nameof(userAnswers));

            _db.UserAnswers.RemoveRange(userAnswers);
            await _db.SaveChangesAsync();
        }

        public async Task<UserAnswer> FindAsync(Predicate<UserAnswer> predicate)
        {
            return await _db.UserAnswers.ExtractAll().FirstOrDefaultAsync(tr => predicate(tr));
        }

        public IQueryable<UserAnswer> GetAll()
        {
            return _db.UserAnswers.ExtractAll();
        }

        public async Task UpdateAsync(UserAnswer userAnswer)
        {
            if (userAnswer is null)
                throw new ArgumentNullException(nameof(userAnswer));

            _db.Entry(userAnswer).State = EntityState.Modified;
            await _db.SaveChangesAsync();
        }
    }
}
