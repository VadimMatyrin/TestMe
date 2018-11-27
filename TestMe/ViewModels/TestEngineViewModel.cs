using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TestMe.Models;

namespace TestMe.ViewModels
{
    public class TestEngineViewModel
    {
        public Test Test { get; set; }
        public TestResult UserTestResult { get; set; }
    }
}
