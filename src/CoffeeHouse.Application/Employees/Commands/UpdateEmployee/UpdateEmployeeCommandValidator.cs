using FluentValidation;

namespace CoffeeHouse.Application.Employees.Commands.UpdateEmployee;

/// <summary>
/// Validator for UpdateEmployeeCommand
/// </summary>
public class UpdateEmployeeCommandValidator : AbstractValidator<UpdateEmployeeCommand>
{
    /// <summary>
    /// Initializes validation rules for UpdateEmployeeCommand
    /// </summary>
    public UpdateEmployeeCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithMessage("Employee ID must be greater than zero");

        RuleFor(x => x.CoffeeShopId)
            .GreaterThan(0)
            .WithMessage("Coffee shop ID must be greater than zero");

        RuleFor(x => x.FullName)
            .NotEmpty()
            .WithMessage("Employee full name is required")
            .MaximumLength(255)
            .WithMessage("Employee full name must not exceed 255 characters");

        RuleFor(x => x.Position)
            .NotEmpty()
            .WithMessage("Employee position is required")
            .MaximumLength(50)
            .WithMessage("Employee position must not exceed 50 characters");

        RuleFor(x => x.PhoneNumber)
            .NotEmpty()
            .WithMessage("Phone number is required")
            .MaximumLength(20)
            .WithMessage("Phone number must not exceed 20 characters")
            .Matches(@"^\d+$")
            .WithMessage("Phone number must contain only digits");

        RuleFor(x => x.IdCardNumber)
            .NotEmpty()
            .WithMessage("ID card number is required")
            .MaximumLength(50)
            .WithMessage("ID card number must not exceed 50 characters");

        RuleFor(x => x.BaseSalary)
            .GreaterThan(0)
            .WithMessage("Base salary must be greater than zero")
            .LessThanOrEqualTo(999999999.99m)
            .WithMessage("Base salary exceeds maximum allowed value");

        RuleFor(x => x.SalaryCoefficient)
            .GreaterThan(0)
            .WithMessage("Salary coefficient must be greater than zero")
            .LessThanOrEqualTo(99.99m)
            .WithMessage("Salary coefficient exceeds maximum allowed value (99.99)");

        RuleFor(x => x.Email)
            .EmailAddress()
            .When(x => !string.IsNullOrWhiteSpace(x.Email))
            .WithMessage("Invalid email format")
            .MaximumLength(255)
            .When(x => !string.IsNullOrWhiteSpace(x.Email))
            .WithMessage("Email must not exceed 255 characters");

        RuleFor(x => x.DateOfBirth)
            .Must(BeValidAge)
            .When(x => x.DateOfBirth.HasValue)
            .WithMessage("Employee must be at least 18 years old and date of birth must be valid");

        RuleFor(x => x.Address)
            .MaximumLength(500)
            .When(x => !string.IsNullOrWhiteSpace(x.Address))
            .WithMessage("Address must not exceed 500 characters");
    }

    /// <summary>
    /// Validates that the date of birth represents a valid age (18-100 years old)
    /// </summary>
    private static bool BeValidAge(DateTime? dateOfBirth)
    {
        if (!dateOfBirth.HasValue)
            return true;

        var age = DateTime.UtcNow.Year - dateOfBirth.Value.Year;
        if (dateOfBirth.Value > DateTime.UtcNow.AddYears(-age))
        {
            age--;
        }

        return age >= 18 && age <= 100;
    }
}
