using FluentValidation;

namespace CoffeeHouse.Application.Employees.Queries.GetEmployeeById;

/// <summary>
/// Validator for GetEmployeeByIdQuery
/// </summary>
public class GetEmployeeByIdQueryValidator : AbstractValidator<GetEmployeeByIdQuery>
{
    /// <summary>
    /// Initializes validation rules for GetEmployeeByIdQuery
    /// </summary>
    public GetEmployeeByIdQueryValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithMessage("Employee ID must be greater than zero");
    }
}
