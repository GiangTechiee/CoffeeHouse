using FluentValidation;

namespace CoffeeHouse.Application.Orders.Queries.GetOrders;

/// <summary>
/// Validator for GetOrdersQuery
/// Validates: Requirements 12.1, 12.2
/// </summary>
public class GetOrdersQueryValidator : AbstractValidator<GetOrdersQuery>
{
    public GetOrdersQueryValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThan(0).WithMessage("Page number must be greater than 0");

        RuleFor(x => x.PageSize)
            .GreaterThan(0).WithMessage("Page size must be greater than 0")
            .LessThanOrEqualTo(100).WithMessage("Page size must not exceed 100");

        RuleFor(x => x.CustomerId)
            .GreaterThan(0).WithMessage("Customer ID must be greater than 0")
            .When(x => x.CustomerId.HasValue);

        RuleFor(x => x.EmployeeId)
            .GreaterThan(0).WithMessage("Employee ID must be greater than 0")
            .When(x => x.EmployeeId.HasValue);

        RuleFor(x => x.CoffeeShopId)
            .GreaterThan(0).WithMessage("Coffee shop ID must be greater than 0")
            .When(x => x.CoffeeShopId.HasValue);

        RuleFor(x => x.Status)
            .Must(status => string.IsNullOrWhiteSpace(status) || 
                           new[] { "Pending", "Confirmed", "Completed", "Cancelled" }.Contains(status))
            .WithMessage("Status must be one of: Pending, Confirmed, Completed, Cancelled")
            .When(x => !string.IsNullOrWhiteSpace(x.Status));

        RuleFor(x => x)
            .Must(x => !x.StartDate.HasValue || !x.EndDate.HasValue || x.StartDate <= x.EndDate)
            .WithMessage("Start date must be before or equal to end date")
            .When(x => x.StartDate.HasValue && x.EndDate.HasValue);
    }
}
