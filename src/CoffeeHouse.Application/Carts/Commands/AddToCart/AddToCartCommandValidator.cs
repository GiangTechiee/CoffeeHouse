using FluentValidation;

namespace CoffeeHouse.Application.Carts.Commands.AddToCart;

/// <summary>
/// Validator for AddToCartCommand
/// </summary>
public class AddToCartCommandValidator : AbstractValidator<AddToCartCommand>
{
    /// <summary>
    /// Initializes a new instance of AddToCartCommandValidator
    /// </summary>
    public AddToCartCommandValidator()
    {
        RuleFor(x => x.CustomerId)
            .GreaterThan(0)
            .WithMessage("Customer ID must be greater than 0");

        RuleFor(x => x.ProductId)
            .GreaterThan(0)
            .WithMessage("Product ID must be greater than 0");

        RuleFor(x => x.Quantity)
            .GreaterThan(0)
            .WithMessage("Quantity must be greater than 0")
            .LessThanOrEqualTo(100)
            .WithMessage("Quantity cannot exceed 100 items");
    }
}
