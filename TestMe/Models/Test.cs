using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
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
        [Display(Name = "Test code")]
        public string TestCode { get; set; }
        [Display(Name = "Test duration")]
        [Required, DataType(DataType.Time)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:hh\\:mm}")]
        [Range(typeof(TimeSpan), "00:01:00", "23:59:00")]
        public TimeSpan TestDuration { get; set; }
        [JsonIgnore]
        public AppUser AppUser { get; set; }
        [NotMapped]
        public int TestScore { get; set; }
        [JsonIgnore]
        public virtual ICollection<TestQuestion> TestQuestions { get; set; }
        [JsonIgnore]
        public virtual ICollection<TestAnswer> TestAnswers { get; set; }
        [JsonIgnore]
        public virtual ICollection<TestResult> TestResults { get; set; }
        [JsonIgnore]
        public virtual ICollection<TestReport> TestReports { get; set; }
        [JsonIgnore]
        public virtual ICollection<TestMark> TestMarks { get; set; }
    }
}
