using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using belt_exam.Validations;

namespace belt_exam.Models {
    public class User {

        [Key]
        public int UserId { get; set; }

        [Required (ErrorMessage = "Enter your first name")]
        [MinLength (2, ErrorMessage = "First name must be at least 2 characters")]
        [RegularExpression (@"^[a-zA-Z]+$", ErrorMessage = "first name must be letters only")]
        public string FirstName { get; set; }

        [Required (ErrorMessage = "Enter your last name")]
        [MinLength (2, ErrorMessage = "Last name must be at least 2 characters")]
        [RegularExpression (@"^[a-zA-Z]+$", ErrorMessage = "Last name must be letters only")]
        public string LastName { get; set; }

        [Required (ErrorMessage = "Enter your email")]
        [EmailAddress (ErrorMessage = "Enter a valid email address")]
        public string Email { get; set; }

        [DataType (DataType.Date)]
        [Eighteen]
        public DateTime Birthday { get; set; }

        [DataType (DataType.Password)]
        [Required (ErrorMessage = "Enter a password")]
        [MinLength (8, ErrorMessage = "Password must be 8 characters or longer!")]
        [RegularExpression(@"^(?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[^\da-zA-Z])(.{8,15})$", ErrorMessage = "Password must meet requirements")]
        public string Password { get; set; }

        [NotMapped]
        [Compare ("Password", ErrorMessage = "Passwords do not match")]
        [DataType (DataType.Password)]
        public string Confirm { get; set; }

        public List<Activity> CreatedActivities {get;set;}

        public List<Association> JoinedActivites { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        public int Age () {
            return DateTime.Now.Year - Birthday.Year;
        }
    }
}