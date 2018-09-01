using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TestMe.Models
{
    public class TestAnswer
    {
        public int Id { get; set; }
        [Required]
        [StringLength(1000)]
        [Display(Name = "Answer")]
        public string AnswerText { get; set; }
        [Display(Name = "Is correct?")]
        [JsonIgnore]
        public bool IsCorrect { get; set; }
        public int TestQuestionId { get; set; }
        [JsonIgnore]
        public TestQuestion TestQuestion { get; set; }
        [Display(Name = "Image")]
        public string ImageName { get; set; }
        [JsonIgnore]
        public string AppUserId { get; set; }

        [JsonIgnore]
        public AppUser AppUser { get; set; }
    }
}
