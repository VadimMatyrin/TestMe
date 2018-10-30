using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TestMe.Models
{
    public class UserAnswer
    {
        public int Id { get; set; }
        [Required]
        public DateTime AnswerTime { get; set; }
        public int TestAnswerId { get; set; }
        public string AppUserId { get; set; }
        public AppUser AppUser { get; set; }
        public TestAnswer TestAnswer { get; set; }
    }
}
