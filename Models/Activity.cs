using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using belt_exam.Validations;

namespace belt_exam.Models {
    public class Activity {

        [Key]
        public int ActivityId { get; set; }

        [Required (ErrorMessage = "Enter your activity")]
        public string Title { get; set; }

        [Required (ErrorMessage = "Enter your activity's description")]
        public string Description { get; set; }

        [DataType (DataType.Date)]
        [CurrentDate]
        public DateTime Date { get; set; }

        [DataType (DataType.Time)]
        public DateTime Time { get; set; }

        public int Duration { get; set; }

        public string Durationstr { get; set; }

        public string Creator {get;set;}

        public List<Association> Participants { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime UpdatedAt { get; set; } = DateTime.Now;

    }
}