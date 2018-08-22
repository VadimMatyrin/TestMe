﻿using Newtonsoft.Json;
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
        public string AnswerText { get; set; }
        [JsonIgnore]
        public bool IsCorrect { get; set; }
        public int TestQuestionId { get; set; }
        [JsonIgnore]
        public TestQuestion TestQuestion { get; set; }

        [JsonIgnore]
        public string AppUserId { get; set; }

        [JsonIgnore]
        public AppUser AppUser { get; set; }
    }
}
