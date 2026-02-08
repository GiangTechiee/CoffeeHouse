using FluentValidation;

namespace CoffeeHouse.Application.Orders.Queries.GetOrderById;

/// <summary>
/// Validator for GetOrderByIdQuery
/// Validates: Requirements 12.1, 12.2
/// </summary>
public class GetOrderByIdQueryValidator : AbstractValidator<GetOrderByIdQuery>
{
    public GetOrderByIdQueryValidator()
    {
        RuleFor(x => x.OrderId)
            .NotEmpty().WithMessage("Order ID must be greater than 0");
    }
}
