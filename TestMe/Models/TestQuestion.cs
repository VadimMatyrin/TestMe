using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TestMe.Models
{
    public class TestQuestion
    {
        public int Id { get; set; }
        [Required]
        [StringLength(1000)]
        [Display(Name = "Question")]
        public string QuestionText { get; set; }
        [StringLength(10000)]
        [Display(Name = "Preformatted text")]
        public string PreformattedText { get; set; }
        [JsonIgnore]
        public int TestId { get; set; }
        public Test Test { get; set; }
        [JsonIgnore]
        public string AppUserId { get; set; }
        [JsonIgnore]
        public AppUser AppUser { get; set; }
        public virtual ICollection<TestAnswer> TestAnswers { get; set; }
    }
}
