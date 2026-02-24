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
        _logger.LogInformation("Registering new customer: {Email}", request.Email);

        if (request.Password != request.ConfirmPassword)
        {
            return new AuthResultDto { Success = false, Message = "Mật khẩu xác nhận không khớp" };
        }

        var existingUser = await _userManager.FindByNameAsync(request.Email);
        if (existingUser != null)
        {
            return new AuthResultDto { Success = false, Message = "Email này đã được sử dụng" };
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
                UserName = request.Email,
                Email = request.Email, // Renamed from Username
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
                return new AuthResultDto { Success = false, Message = "Đăng ký không thành công. Vui lòng kiểm tra lại thông tin." };
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
                return new AuthResultDto { Success = false, Message = "Không thể gán quyền người dùng" };
            }

            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            _logger.LogInformation("Customer {Email} registered successfully", request.Email);

            return new AuthResultDto
            {
                Success = true,
                Email = user.UserName ?? string.Empty,
                Role = "User",
                CustomerId = customer.CustomerId,
                Message = "Registration successful"
            };
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            _logger.LogError(ex, "Error occurred while registering customer {Email}", request.Email);
            throw;
        }
    }
}
