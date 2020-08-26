using System;
using System.ComponentModel.DataAnnotations;

namespace belt_exam.Validations {
    public class EighteenAttribute : ValidationAttribute {
        protected override ValidationResult IsValid (object value, ValidationContext validationContext) {

            if (value is DateTime) {
                DateTime check = (DateTime) value;
                if (DateTime.Now.AddYears (-18) > check) {
                    return ValidationResult.Success;
                } else {
                    return new ValidationResult ("To young to be making plans");
                }
            } else {
                return new ValidationResult ("Please enter a Valid Date.");
            }

        }
    }

}