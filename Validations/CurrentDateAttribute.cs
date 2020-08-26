using System;
using System.ComponentModel.DataAnnotations;

namespace belt_exam.Validations
{
    public class CurrentDateAttribute : ValidationAttribute
    {

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            DateTime dt = (DateTime)value;
            if(dt >= DateTime.Now)
            {
                return ValidationResult.Success;
            }
            return new ValidationResult (ErrorMessage ?? "Sorry no time travel");
        }
    }
}