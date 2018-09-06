using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TestMe.Models
{
    public class TestMark
    {
        public int Id { get; set; }
        [Required]
        public bool EnjoyedTest { get; set; }
        public int TestId { get; set; }
        public Test Test { get; set; }
        public string AppUserId { get; set; }
        public AppUser AppUser { get; set; }
    }
}
