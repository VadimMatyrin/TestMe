using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TestMe.Models
{
    public class TestQuestion
    {
        public int Id { get; set; }
        [Required]
        [StringLength(1000)]
        public string QuestionText { get; set; }
        [Required]
        public int TestId { get; set; }
        public Test Test { get; set; }
        public ICollection<TestAnswer> TestAnswers { get; set; }
    }
}
