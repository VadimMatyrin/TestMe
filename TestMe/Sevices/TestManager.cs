using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TestMe.Data.Interfaces;
using TestMe.Models;
using TestMe.Sevices.Interfaces;

namespace TestMe.Sevices
{
    public class TestManager : ITestManager
    {
        private readonly IRepository<Test> _repository;
        public TestManager(IRepository<Test> repository) => _repository = repository;

        public Task AddAsync(Test test) => _repository.AddAsync(test);

        public Task DeleteAsync(Test test) => _repository.DeleteAsync(test);

        public async Task DeleteRangeAsync(IEnumerable<Test> tests) => await _repository.DeleteRangeAsync(tests);
        public Task<Test> FindAsync(Predicate<Test> predicate) => _repository.FindAsync(predicate);

        public IQueryable<Test> GetAll() => _repository.GetAll();
        public async Task UpdateAsync(Test test) => await _repository.UpdateAsync(test );
        public Test GetTest(string userId, int? id)
        {
            if (userId is null || id is null)
                throw new ArgumentNullException();

            return GetAll().FirstOrDefault(t => t.Id == id && t.AppUserId == userId);
        }
    }
}
