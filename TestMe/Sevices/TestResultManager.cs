using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TestMe.Data.Interfaces;
using TestMe.Models;
using TestMe.Sevices.Interfaces;

namespace TestMe.Sevices
{
    public class TestResultManager : ITestResultManager
    {
        private readonly IRepository<TestResult> _repository;
        public TestResultManager(IRepository<TestResult> repository) => _repository = repository;

        public Task AddAsync(TestResult testResult) => _repository.AddAsync(testResult);

        public Task DeleteAsync(TestResult testResult) => _repository.DeleteAsync(testResult);

        public async Task DeleteRangeAsync(IEnumerable<TestResult> testResults) => await _repository.DeleteRangeAsync(testResults);
        public Task<TestResult> FindAsync(Predicate<TestResult> predicate) => _repository.FindAsync(predicate);

        public IQueryable<TestResult> GetAll() => _repository.GetAll();
        public async Task UpdateAsync(TestResult testResult) => await _repository.UpdateAsync(testResult);
        public TestResult GetTestResult(string userId, int? testId, int? id)
        {
            if (userId is null || testId is null || id is null)
                throw new ArgumentNullException();

            return GetAll().FirstOrDefault(tr => tr.Id == id && tr.TestId == testId && tr.Test.AppUserId == userId);
        }
    }
}
