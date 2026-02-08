using MediatR;

namespace CoffeeHouse.Application.Customers.Commands.CreateCustomer;

/// <summary>
/// Command for creating a new customer
/// </summary>
public record CreateCustomerCommand(
    string Name,
    string PhoneNumber,
    string Address) : IRequest<int>;
