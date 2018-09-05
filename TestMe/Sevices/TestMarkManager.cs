using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TestMe.Data.Interfaces;
using TestMe.Models;
using TestMe.Sevices.Interfaces;

namespace TestMe.Sevices
{
    public class TestMarkManager : ITestMarkManager
    {
        private readonly IRepository<TestMark> _repository;
        public TestMarkManager(IRepository<TestMark> repository) => _repository = repository;

        public Task AddAsync(TestMark testMark) => _repository.AddAsync(testMark);

        public Task DeleteAsync(TestMark testMark) => _repository.DeleteAsync(testMark);

        public async Task DeleteRangeAsync(IEnumerable<TestMark> testMarks) => await _repository.DeleteRangeAsync(testMarks);
        public Task<TestMark> FindAsync(Predicate<TestMark> predicate) => _repository.FindAsync(predicate);

        public IQueryable<TestMark> GetAll() => _repository.GetAll();
        public async Task UpdateAsync(TestMark testMarks) => await _repository.UpdateAsync(testMarks);
    }
}
