using CoffeeHouse.Domain.Exceptions;
using CoffeeHouse.Domain.Interfaces;
using CoffeeHouse.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;
using BCrypt.Net;

namespace CoffeeHouse.Application.Authentication.Commands.UpdateAccountInfo;

/// <summary>
/// Handler for UpdateAccountInfoCommand
/// </summary>
public class UpdateAccountInfoCommandHandler : IRequestHandler<UpdateAccountInfoCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UpdateAccountInfoCommandHandler> _logger;
    private readonly IPasswordHasher _passwordHasher;

    public UpdateAccountInfoCommandHandler(
        IUnitOfWork unitOfWork, 
        ILogger<UpdateAccountInfoCommandHandler> logger,
        IPasswordHasher passwordHasher)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _passwordHasher = passwordHasher;
    }

    public async Task<bool> Handle(UpdateAccountInfoCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating account info for username: {Username}", request.CurrentUsername);

        // Get account with related entities
        var account = await _unitOfWork.Accounts.GetByUsernameAsync(request.CurrentUsername, cancellationToken);
        
        if (account == null)
        {
             _logger.LogWarning("Account not found: {Username}", request.CurrentUsername);
             throw new NotFoundException($"Account with username {request.CurrentUsername} not found");
        }

        // Update Account Info
        if (!string.IsNullOrEmpty(request.NewUsername) && request.NewUsername != account.Username)
        {
             // Check if new username exists
             var existing = await _unitOfWork.Accounts.GetByUsernameAsync(request.NewUsername, cancellationToken);
             if (existing != null)
             {
                 throw new InvalidOperationException($"Username '{request.NewUsername}' is already taken.");
             }
             account.Username = request.NewUsername;
        }

        if (!string.IsNullOrEmpty(request.NewPassword))
        {
            account.Password = _passwordHasher.HashPassword(request.NewPassword);
        }

        // Update Profile Info
        if (request.AccountInfo != null)
        {
            if (account.EmployeeId.HasValue && request.AccountInfo.Employee != null)
            {
                var empDto = request.AccountInfo.Employee;
                var employee = account.Employee ?? await _unitOfWork.Employees.GetByIdAsync(account.EmployeeId.Value, cancellationToken);
                
                if (employee != null)
                {
                    employee.FullName = !string.IsNullOrEmpty(empDto.FullName) ? empDto.FullName : employee.FullName;
                    employee.Address = !string.IsNullOrEmpty(empDto.Address) ? empDto.Address : employee.Address;
                    employee.Email = !string.IsNullOrEmpty(empDto.Email) ? empDto.Email : employee.Email;
                    employee.DateOfBirth = empDto.DateOfBirth ?? employee.DateOfBirth;
                    employee.Gender = empDto.Gender ?? employee.Gender;
                    employee.Position = !string.IsNullOrEmpty(empDto.Position) ? empDto.Position : employee.Position;
                    employee.PhoneNumber = !string.IsNullOrEmpty(empDto.PhoneNumber) ? empDto.PhoneNumber : employee.PhoneNumber;
                    employee.IdentityCardNumber = !string.IsNullOrEmpty(empDto.IdCardNumber) ? empDto.IdCardNumber : employee.IdentityCardNumber;
                    employee.BaseSalary = empDto.BaseSalary != 0 ? empDto.BaseSalary : employee.BaseSalary;
                    employee.SalaryCoefficient = empDto.SalaryCoefficient != 0 ? empDto.SalaryCoefficient : employee.SalaryCoefficient;
                    employee.StoreId = empDto.CoffeeShopId != 0 ? empDto.CoffeeShopId : employee.StoreId;

                    _unitOfWork.Employees.Update(employee);
                }
            }
            else if (account.CustomerId.HasValue && request.AccountInfo.Customer != null)
            {
                var custDto = request.AccountInfo.Customer;
                var customer = account.Customer ?? await _unitOfWork.Customers.GetByIdAsync(account.CustomerId.Value, cancellationToken);

                if (customer != null)
                {
                     customer.CustomerName = !string.IsNullOrEmpty(custDto.Name) ? custDto.Name : customer.CustomerName;
                     customer.Address = !string.IsNullOrEmpty(custDto.Address) ? custDto.Address : customer.Address;
                     customer.PhoneNumber = !string.IsNullOrEmpty(custDto.PhoneNumber) ? custDto.PhoneNumber : customer.PhoneNumber;

                     _unitOfWork.Customers.Update(customer);
                }
            }
        }

        _unitOfWork.Accounts.Update(account);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Account updated successfully for username: {Username}", request.CurrentUsername);
        return true;
    }
}

