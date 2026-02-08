using MediatR;
using CoffeeHouse.Application.Common.Models;

namespace CoffeeHouse.Application.Customers.Commands.DeleteCustomer;

/// <summary>
/// Command to delete a customer
/// </summary>
public record DeleteCustomerCommand(int Id) : IRequest<Result>;
