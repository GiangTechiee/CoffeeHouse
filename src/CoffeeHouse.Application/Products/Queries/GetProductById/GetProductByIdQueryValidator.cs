using FluentValidation;

namespace CoffeeHouse.Application.Products.Queries.GetProductById;

/// <summary>
/// Validator for GetProductByIdQuery
/// </summary>
public class GetProductByIdQueryValidator : AbstractValidator<GetProductByIdQuery>
{
    public GetProductByIdQueryValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithMessage("Product ID must be greater than 0");
    }
}
