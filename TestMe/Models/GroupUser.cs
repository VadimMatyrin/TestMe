using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TestMe.Models
{
    public class GroupUser
    {
        public int Id { get; set; }

        public int GroupId { get; set; }

        public Group Group { get; set; }

        public string AppUserId { get; set; }

        public AppUser AppUser { get; set; }

    }
}
