using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TestMe.Models
{
    public class Group
    {
        public int Id { get; set; }
        [Required]
        [StringLength(30, MinimumLength = 10)]
        public string Name { get; set; }

        public string AppUserId { get; set; }

        public AppUser AppUser { get; set; }
    }
}
