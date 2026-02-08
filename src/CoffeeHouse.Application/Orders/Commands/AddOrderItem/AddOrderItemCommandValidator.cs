using FluentValidation;

namespace CoffeeHouse.Application.Orders.Commands.AddOrderItem;

/// <summary>
/// Validator for AddOrderItemCommand
/// Validates: Requirements 12.1, 12.2
/// </summary>
public class AddOrderItemCommandValidator : AbstractValidator<AddOrderItemCommand>
{
    public AddOrderItemCommandValidator()
    {
        RuleFor(x => x.OrderId)
            .NotEmpty().WithMessage("Order ID is required");

        RuleFor(x => x.ProductId)
            .GreaterThan(0).WithMessage("Product ID is required");

        RuleFor(x => x.Quantity)
            .GreaterThan(0).WithMessage("Quantity must be greater than 0")
            .LessThanOrEqualTo(1000).WithMessage("Quantity must not exceed 1000");
    }
}
