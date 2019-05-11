using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TestMe.Models
{
    public class TestGroups
    {
        public int Id { get; set; }
        public int TestId { get; set; }
        public Test Test { get; set; }
        public int GroupId { get; set; }
        public Group Group { get; set; }
    }
}
