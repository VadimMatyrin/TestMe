using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TestMe.Models;

namespace TestMe.ViewModels
{
    public class UsersRecordViewModel
    {
        public AppUser User { get; set; }
        public ICollection<Test> Tests { get; set; }
        public ICollection<TestResult> TestResults { get; set; }
        public double AvgTestMark { get; set; }

    }
}
