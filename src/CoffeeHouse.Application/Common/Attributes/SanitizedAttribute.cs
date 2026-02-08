using CoffeeHouse.Application.Common.Helpers;
using System.ComponentModel.DataAnnotations;

namespace CoffeeHouse.Application.Common.Attributes;

public class SanitizedAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value == null)
        {
            return ValidationResult.Success;
        }

        if (value is string stringValue)
        {
            var sanitized = InputSanitizer.SanitizeAll(stringValue);

            if (sanitized != stringValue)
            {
                return new ValidationResult("Input contains invalid characters.");
            }

            return ValidationResult.Success;
        }

        return new ValidationResult("The Sanitized attribute can only be applied to string properties.");
    }
}
