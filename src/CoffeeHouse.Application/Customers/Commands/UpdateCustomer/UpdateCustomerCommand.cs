using MediatR;

namespace CoffeeHouse.Application.Customers.Commands.UpdateCustomer;

/// <summary>
/// Command for updating an existing customer
/// </summary>
public record UpdateCustomerCommand(
    int Id,
    string Name,
    string PhoneNumber,
    string Address) : IRequest<Unit>;
