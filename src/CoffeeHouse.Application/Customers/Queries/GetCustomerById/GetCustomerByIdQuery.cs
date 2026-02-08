using CoffeeHouse.Application.Common.Models;
using CoffeeHouse.Application.Customers.DTOs;
using MediatR;

namespace CoffeeHouse.Application.Customers.Queries.GetCustomerById;

public class GetCustomerByIdQuery : IRequest<Result<CustomerDto>>
{
    public int Id { get; set; }
}
