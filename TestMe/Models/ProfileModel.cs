using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TestMe.Models
{
    public class ProfileModel
    {
        public AppUser AppUser { get; set; }
        public ICollection<Test> UserTests { get; set; }
        public ICollection<TestResult> TestResults { get; set; }
        public ICollection<TestMark> TestMarks { get; set; }
    }
}
