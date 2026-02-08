using FluentValidation;

namespace CoffeeHouse.Application.Employees.Commands.DeleteEmployee;

/// <summary>
/// Validator for DeleteEmployeeCommand
/// </summary>
public class DeleteEmployeeCommandValidator : AbstractValidator<DeleteEmployeeCommand>
{
    /// <summary>
    /// Initializes validation rules for DeleteEmployeeCommand
    /// </summary>
    public DeleteEmployeeCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithMessage("Employee ID must be greater than zero");
    }
}
