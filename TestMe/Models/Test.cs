using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TestMe.Models
{
    public class Test
    {
        public int Id { get; set; }
        [Required]
        [StringLength(200)]
        public string TestName { get; set; }
        public DateTime CreationDate { get; set; }
        public string AppUserId { get; set; }
        public AppUser AppUser { get; set; }
        public virtual ICollection<TestQuestion> TestQuestions { get; set; }
    }
}
