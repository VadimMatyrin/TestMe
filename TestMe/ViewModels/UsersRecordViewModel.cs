using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TestMe.Models;

namespace TestMe.ViewModels
{
    public class UsersRecordViewModel
    {
        public AppUser User;
        public ICollection<Test> Tests;
        public ICollection<TestResult> TestResults;
    }
}
