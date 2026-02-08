using FluentValidation;

namespace CoffeeHouse.Application.Carts.Commands.Checkout;

/// <summary>
/// Validator for CheckoutCommand
/// </summary>
public class CheckoutCommandValidator : AbstractValidator<CheckoutCommand>
{
    /// <summary>
    /// Initializes a new instance of CheckoutCommandValidator
    /// </summary>
    public CheckoutCommandValidator()
    {
        RuleFor(x => x.CustomerId)
            .GreaterThan(0)
            .WithMessage("Customer ID must be greater than 0");

        RuleFor(x => x.CustomerName)
            .NotEmpty()
            .WithMessage("Customer name is required")
            .MaximumLength(255)
            .WithMessage("Customer name must not exceed 255 characters");

        RuleFor(x => x.PhoneNumber)
            .NotEmpty()
            .WithMessage("Phone number is required")
            .Matches(@"^(\+84|0)[0-9]{9,10}$")
            .WithMessage("Phone number must be a valid Vietnamese phone number");

        RuleFor(x => x.Address)
            .NotEmpty()
            .WithMessage("Address is required")
            .MaximumLength(500)
            .WithMessage("Address must not exceed 500 characters");

        RuleFor(x => x.CoffeeShopId)
            .GreaterThan(0)
            .WithMessage("Coffee shop ID must be greater than 0");

        RuleFor(x => x.PaymentMethod)
            .NotEmpty()
            .WithMessage("Payment method is required")
            .MaximumLength(50)
            .WithMessage("Payment method must not exceed 50 characters");
    }
}
