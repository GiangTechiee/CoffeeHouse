using MediatR;
using CoffeeHouse.Application.Authentication.DTOs;

namespace CoffeeHouse.Application.Authentication.Commands.RegisterCustomer;

public class RegisterCustomerCommand : IRequest<AuthResultDto>
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string ConfirmPassword { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
}
