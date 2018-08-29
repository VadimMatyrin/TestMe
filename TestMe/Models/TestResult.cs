using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TestMe.Models
{
    public class TestResult
    {
        public int Id { get; set; }
        [Required]
        public int Score { get; set; }
        [Required]
        public string Username { get; set; }
        [Required]
        public DateTime StartTime { get; set; }
        [Required]
        public DateTime FinishTime { get; set; }
        public int TestId { get; set; }
        public Test Test;
        //public string AppUserId { get; set; }
        //public AppUser AppUser { get; set; }
    }
}
