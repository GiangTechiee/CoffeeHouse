using CoffeeHouse.Application.Authentication.DTOs;
using CoffeeHouse.Domain.Entities;
using CoffeeHouse.Domain.Identity;
using CoffeeHouse.Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace CoffeeHouse.Application.Authentication.Commands.RegisterCustomer;

public class RegisterCustomerCommandHandler : IRequestHandler<RegisterCustomerCommand, AuthResultDto>
{
    private readonly UserManager<AppUser> _userManager;
    private readonly RoleManager<AppRole> _roleManager;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<RegisterCustomerCommandHandler> _logger;

    public RegisterCustomerCommandHandler(
        UserManager<AppUser> userManager,
        RoleManager<AppRole> roleManager,
        IUnitOfWork unitOfWork,
        ILogger<RegisterCustomerCommandHandler> logger)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<AuthResultDto> Handle(RegisterCustomerCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Registering new customer: {Username}", request.Username);

        if (request.Password != request.ConfirmPassword)
        {
            return new AuthResultDto { Success = false, Message = "Passwords do not match" };
        }

        var existingUser = await _userManager.FindByNameAsync(request.Username);
        if (existingUser != null)
        {
            return new AuthResultDto { Success = false, Message = "Username already exists" };
        }

        try 
        {
            await _unitOfWork.BeginTransactionAsync(cancellationToken);

            // Create Customer entity first
            var customer = new Customer
            {
                CustomerName = request.FullName,
                PhoneNumber = request.PhoneNumber,
                Address = request.Address
            };

            await _unitOfWork.Customers.AddAsync(customer, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Create Identity User
            var user = new AppUser
            {
                UserName = request.Username,
                Email = request.Username, // Validated as email by RegisterCustomerCommandValidator
                FullName = request.FullName,
                Address = request.Address,
                CustomerId = customer.CustomerId,
                CreatedAt = DateTime.UtcNow
            };

            var result = await _userManager.CreateAsync(user, request.Password);

            if (!result.Succeeded)
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                _logger.LogError("User creation failed: {Errors}", string.Join(", ", result.Errors.Select(e => e.Description)));
                return new AuthResultDto { Success = false, Message = result.Errors.First().Description };
            }

            // Assign "User" role
            if (!await _roleManager.RoleExistsAsync("User"))
            {
                await _roleManager.CreateAsync(new AppRole { Name = "User", Description = "Default customer role" });
            }

            var roleResult = await _userManager.AddToRoleAsync(user, "User");
            if (!roleResult.Succeeded)
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                _logger.LogError("User role assignment failed: {Errors}", string.Join(", ", roleResult.Errors.Select(e => e.Description)));
                return new AuthResultDto { Success = false, Message = "Role assignment failed" };
            }

            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            _logger.LogInformation("Customer {Username} registered successfully", request.Username);

            return new AuthResultDto
            {
                Success = true,
                Username = user.UserName,
                Role = "User",
                CustomerId = customer.CustomerId,
                Message = "Registration successful"
            };
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            _logger.LogError(ex, "Error occurred while registering customer {Username}", request.Username);
            throw;
        }
    }
}
