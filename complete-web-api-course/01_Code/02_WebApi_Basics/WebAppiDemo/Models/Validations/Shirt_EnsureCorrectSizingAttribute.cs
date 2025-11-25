using System.ComponentModel.DataAnnotations;
using System.Drawing;

namespace WebAppiDemo.Models.Validations
{
    public class Shirt_EnsureCorrectSizingAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var shirt = validationContext.ObjectInstance as Shirt;
            if (shirt != null && !string.IsNullOrWhiteSpace(shirt.Gender) && shirt.Size.HasValue)
            {
                if (shirt.Gender.Equals("Male", StringComparison.OrdinalIgnoreCase) && shirt.Size < 8)
                {
                    return new ValidationResult("Invalid size for male shirt. Minimum size is 8.");
                }
                else if (shirt.Gender.Equals("Female", StringComparison.OrdinalIgnoreCase) && shirt.Size < 6)
                {
                    return new ValidationResult("Invalid size for female shirt. Minimum size is 6.");
                }
            }
            return ValidationResult.Success;
        }
    }
}
