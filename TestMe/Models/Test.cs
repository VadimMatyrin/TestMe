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
        [Display(Name = "Test name")]
        [StringLength(200)]
        public string TestName { get; set; }
        [JsonIgnore]
        [Display(Name = "Creation date")]
        public DateTime CreationDate { get; set; }
        [JsonIgnore]
        public string AppUserId { get; set; }
        public string TestCode { get; set; }
        [Display(Name = "Test duration")]
        [Required, DataType(DataType.Time)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:hh\\:mm}")]
        [RegularExpression(@"((([0-1][0-9])|(2[0-3]))(:[0-5][0-9])(:[0-5][0-9])?)", ErrorMessage = "Duration must be between 00:00 to 23:59")]
        public TimeSpan TestDuration { get; set; }
        [JsonIgnore]
        public AppUser AppUser { get; set; }
        [JsonIgnore]
        public virtual ICollection<TestQuestion> TestQuestions { get; set; }
        [JsonIgnore]
        public virtual ICollection<TestResult> TestResults { get; set; }
    }
}
