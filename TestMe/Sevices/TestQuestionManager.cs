using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TestMe.Data.Interfaces;
using TestMe.Models;
using TestMe.Sevices.Interfaces;

namespace TestMe.Sevices
{
    public class TestQuestionManager : ITestQuestionManager
    {
        private readonly IRepository<TestQuestion> _repository;
        public TestQuestionManager(IRepository<TestQuestion> repository) => _repository = repository;

        public Task AddAsync(TestQuestion testQuestion) => _repository.AddAsync(testQuestion);

        public Task DeleteAsync(TestQuestion testQuestion) => _repository.DeleteAsync(testQuestion);

        public async Task DeleteRangeAsync(IEnumerable<TestQuestion> testQuestions) => await _repository.DeleteRangeAsync(testQuestions);
        public Task<TestQuestion> FindAsync(Predicate<TestQuestion> predicate) => _repository.FindAsync(predicate);

        public IQueryable<TestQuestion> GetAll() => _repository.GetAll();
        public async Task UpdateAsync(TestQuestion testQuestion) => await _repository.UpdateAsync(testQuestion);
        public Task<TestQuestion> GetTestQuestionAsync(string userId, int? id)
        {
            if (userId is null || id is null)
                throw new ArgumentNullException();

            return Task.Run(() =>
            {
                return GetAll().FirstOrDefault(tq => tq.Id == id && tq.AppUserId == userId);
            });
        }
    }
}
