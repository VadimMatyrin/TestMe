using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TestMe.Models
{
    public class TestQuestion
    {
        public int Id { get; set; }
        public string QuestionText { get; set; }
        public Test Test { get; set; }
        public ICollection<TestAnswer> TestAnswers { get; set; }
    }
}
