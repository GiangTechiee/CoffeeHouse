using FluentValidation;

namespace CoffeeHouse.Application.Orders.Commands.RemoveOrderItem;

/// <summary>
/// Validator for RemoveOrderItemCommand
/// Validates: Requirements 12.1, 12.2
/// </summary>
public class RemoveOrderItemCommandValidator : AbstractValidator<RemoveOrderItemCommand>
{
    public RemoveOrderItemCommandValidator()
    {
        RuleFor(v => v.OrderId)
            .NotEmpty().WithMessage("Order ID is required");

        RuleFor(x => x.ProductId)
            .GreaterThan(0).WithMessage("Product ID is required");
    }
}
