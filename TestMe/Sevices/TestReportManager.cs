using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TestMe.Data.Interfaces;
using TestMe.Models;
using TestMe.Sevices.Interfaces;

namespace TestMe.Sevices.Extentions
{
    public class TestReportManager : ITestReportManager
    {
        private readonly IRepository<TestReport> _repository;
        public TestReportManager(IRepository<TestReport> repository) => _repository = repository;

        public Task AddAsync(TestReport testReport) => _repository.AddAsync(testReport);

        public Task DeleteAsync(TestReport testReport) => _repository.DeleteAsync(testReport);

        public async Task DeleteRangeAsync(IEnumerable<TestReport> testReports) => await _repository.DeleteRangeAsync(testReports);
        public Task<TestReport> FindAsync(Predicate<TestReport> predicate) => _repository.FindAsync(predicate);

        public IQueryable<TestReport> GetAll() => _repository.GetAll();
        public async Task UpdateAsync(TestReport testResult) => await _repository.UpdateAsync(testResult);
    }
}
