using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TestMe.Data.Interfaces;
using TestMe.Models;
using TestMe.Sevices.Interfaces;

namespace TestMe.Sevices
{
    public class TestAnswerManager : ITestAnswerManager
    {
        private readonly IRepository<TestAnswer> _repository;
        public TestAnswerManager(IRepository<TestAnswer> repository) => _repository = repository;

        public Task AddAsync(TestAnswer testAnswer) => _repository.AddAsync(testAnswer);

        public Task DeleteAsync(TestAnswer testAnswer) => _repository.DeleteAsync(testAnswer);

        public async Task DeleteRangeAsync(IEnumerable<TestAnswer> testAnswers) => await _repository.DeleteRangeAsync(testAnswers);
        public Task<TestAnswer> FindAsync(Predicate<TestAnswer> predicate) => _repository.FindAsync(predicate);

        public IQueryable<TestAnswer> GetAll() => _repository.GetAll();
        public async Task UpdateAsync(TestAnswer testAnswer) => await _repository.UpdateAsync(testAnswer);
    }
}
