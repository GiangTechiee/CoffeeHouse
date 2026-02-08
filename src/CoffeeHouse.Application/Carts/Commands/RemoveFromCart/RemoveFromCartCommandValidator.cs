using FluentValidation;

namespace CoffeeHouse.Application.Carts.Commands.RemoveFromCart;

/// <summary>
/// Validator for RemoveFromCartCommand
/// </summary>
public class RemoveFromCartCommandValidator : AbstractValidator<RemoveFromCartCommand>
{
    /// <summary>
    /// Initializes a new instance of RemoveFromCartCommandValidator
    /// </summary>
    public RemoveFromCartCommandValidator()
    {
        RuleFor(x => x.CustomerId)
            .GreaterThan(0)
            .WithMessage("Customer ID must be greater than 0");

        RuleFor(x => x.ProductId)
            .GreaterThan(0)
            .WithMessage("Product ID must be greater than 0");
    }
}
