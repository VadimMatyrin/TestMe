using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TestMe.Models
{
    public class Test
    {
        [JsonIgnore]
        public int Id { get; set; }
        [Required]
        [StringLength(200)]
        public string TestName { get; set; }
        [JsonIgnore]
        public DateTime CreationDate { get; set; }
        [JsonIgnore]
        public string AppUserId { get; set; }
        public string TestCode { get; set; }
        [JsonIgnore]
        public AppUser AppUser { get; set; }
        [JsonIgnore]
        public virtual ICollection<TestQuestion> TestQuestions { get; set; }
        [JsonIgnore]
        public virtual ICollection<TestResult> TestResults { get; set; }
    }
}
