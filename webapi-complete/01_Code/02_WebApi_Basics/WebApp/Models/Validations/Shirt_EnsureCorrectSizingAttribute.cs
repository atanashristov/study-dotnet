using System.ComponentModel.DataAnnotations;

namespace WebApp.Models.Validations
{
    // Interface for types that need size validation
    public interface IShirtSizing
    {
        string? Gender { get; }
        int? Size { get; }
    }

    public class Shirt_EnsureCorrectSizingAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            // Cast to interface - no reflection needed
            var shirtSizing = validationContext.ObjectInstance as IShirtSizing;

            if (shirtSizing != null)
            {
                if (!ShirtSizeValidator.IsValidSize(shirtSizing.Gender, shirtSizing.Size, out var errorMessage))
                {
                    return new ValidationResult(errorMessage);
                }
            }

            return ValidationResult.Success;
        }
    }
}
