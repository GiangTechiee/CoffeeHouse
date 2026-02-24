using AutoMapper;
using CoffeeHouse.Application.Authentication.DTOs;
using CoffeeHouse.Domain.Entities;
using CoffeeHouse.Application.LegacyAccounts.Specifications;
using CoffeeHouse.Domain.Exceptions;
using CoffeeHouse.Domain.Interfaces;
using MediatR;

namespace CoffeeHouse.Application.LegacyAccounts.Commands.CreateLegacyAccount;

public class CreateLegacyAccountCommandHandler : IRequestHandler<CreateLegacyAccountCommand, AccountInfoDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CreateLegacyAccountCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<AccountInfoDto> Handle(CreateLegacyAccountCommand request, CancellationToken cancellationToken)
    {
        if (await _unitOfWork.Accounts.ExistsAsync(request.Username, cancellationToken))
        {
            throw new InvalidOperationException($"Username {request.Username} already exists");
        }

        var role = await _unitOfWork.Roles.GetByIdAsync(request.RoleId, cancellationToken);
        if (role == null)
        {
            throw new NotFoundException($"Role with ID {request.RoleId} not found");
        }

        if (request.EmployeeId.HasValue)
        {
            var employee = await _unitOfWork.Employees.GetByIdAsync(request.EmployeeId.Value, cancellationToken);
            if (employee == null)
            {
                throw new NotFoundException($"Employee with ID {request.EmployeeId.Value} not found");
            }
        }

        if (request.CustomerId.HasValue)
        {
            var customer = await _unitOfWork.Customers.GetByIdAsync(request.CustomerId.Value, cancellationToken);
            if (customer == null)
            {
                throw new NotFoundException($"Customer with ID {request.CustomerId.Value} not found");
            }
        }

        var account = new Account
        {
            Username = request.Username.Trim(),
            Password = request.Password,
            RoleId = request.RoleId,
            Status = string.IsNullOrWhiteSpace(request.Status) ? "Active" : request.Status.Trim(),
            EmployeeId = request.EmployeeId,
            CustomerId = request.CustomerId
        };

        await _unitOfWork.Accounts.AddAsync(account, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var loaded = await _unitOfWork.Accounts.FirstOrDefaultAsync(
            new LegacyAccountByIdSpec(account.AccountId),
            cancellationToken);
        if (loaded == null)
        {
            throw new NotFoundException("Account not found after creation");
        }

        return _mapper.Map<AccountInfoDto>(loaded);
    }
}
