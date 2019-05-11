using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TestMe.Data.Interfaces;
using TestMe.Models;

namespace TestMe.Sevices
{
    public class GroupUserManager : IRepository<GroupUser>
    {
        private readonly IRepository<GroupUser> _repository;
        public GroupUserManager(IRepository<GroupUser> repository) => _repository = repository;

        public Task AddAsync(GroupUser test) => _repository.AddAsync(test);

        public Task DeleteAsync(GroupUser test) => _repository.DeleteAsync(test);

        public async Task DeleteRangeAsync(IEnumerable<GroupUser> tests) => await _repository.DeleteRangeAsync(tests);
        public Task<GroupUser> FindAsync(Predicate<GroupUser> predicate) => _repository.FindAsync(predicate);

        public IQueryable<GroupUser> GetAll() => _repository.GetAll();
        public async Task UpdateAsync(GroupUser test) => await _repository.UpdateAsync(test);
    }
}
