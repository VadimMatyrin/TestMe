using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TestMe.Data.Interfaces;
using TestMe.Models;
using TestMe.Sevices.Interfaces;

namespace TestMe.Sevices
{
    public class UserAnswerManager: IUserAnswerManager
    {
        private readonly IRepository<UserAnswer> _repository;
        public UserAnswerManager(IRepository<UserAnswer> repository) => _repository = repository;

        public Task AddAsync(UserAnswer userAnswer) => _repository.AddAsync(userAnswer);

        public Task DeleteAsync(UserAnswer userAnswer) => _repository.DeleteAsync(userAnswer);

        public async Task DeleteRangeAsync(IEnumerable<UserAnswer> userAnswers) => await _repository.DeleteRangeAsync(userAnswers);
        public Task<UserAnswer> FindAsync(Predicate<UserAnswer> predicate) => _repository.FindAsync(predicate);

        public IQueryable<UserAnswer> GetAll() => _repository.GetAll();
        public async Task UpdateAsync(UserAnswer userAnswer) => await _repository.UpdateAsync(userAnswer);
    }
}
