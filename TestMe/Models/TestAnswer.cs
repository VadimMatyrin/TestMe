﻿using System;
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
        public bool IsCorrect { get; set; }
        [Required]
        public int TestQuestionId { get; set; }
        public TestQuestion TestQuestion { get; set; }
    }
}
