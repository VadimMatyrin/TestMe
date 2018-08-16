using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TestMe.Models
{
    public class Test
    {
        public int Id { get; set; }
        public DateTime CreationDate { get; set; }
        public AppUser AppUser { get; set; }
        public virtual ICollection<TestQuestion> TestQuestions { get; set; }
    }
}
