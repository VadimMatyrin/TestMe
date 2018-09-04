using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TestMe.Models
{
    public class TestReport
    {
        public int Id { get; set; }
        [Required]
        [StringLength(2000)]
        public string Message { get; set; }
        public int TestId { get; set; }
        public Test Test { get; set; }
        public string AppUserId { get; set; }
        public AppUser AppUser { get; set; }
    }
}
