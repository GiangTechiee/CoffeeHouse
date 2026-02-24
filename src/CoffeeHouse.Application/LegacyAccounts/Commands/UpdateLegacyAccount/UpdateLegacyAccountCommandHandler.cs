using AutoMapper;
using CoffeeHouse.Application.Authentication.DTOs;
using CoffeeHouse.Application.LegacyAccounts.Specifications;
using CoffeeHouse.Domain.Exceptions;
using CoffeeHouse.Domain.Interfaces;
using MediatR;

namespace CoffeeHouse.Application.LegacyAccounts.Commands.UpdateLegacyAccount;

public class UpdateLegacyAccountCommandHandler : IRequestHandler<UpdateLegacyAccountCommand, AccountInfoDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public UpdateLegacyAccountCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<AccountInfoDto> Handle(UpdateLegacyAccountCommand request, CancellationToken cancellationToken)
    {
        var account = await _unitOfWork.Accounts.GetByIdAsync(request.AccountId, cancellationToken);
        if (account == null)
        {
            throw new NotFoundException($"Account with ID {request.AccountId} not found");
        }

        if (!string.Equals(account.Username, request.Username, StringComparison.OrdinalIgnoreCase))
        {
            if (await _unitOfWork.Accounts.ExistsAsync(request.Username, cancellationToken))
            {
                throw new InvalidOperationException($"Username {request.Username} already exists");
            }
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

        account.Username = request.Username.Trim();
        account.RoleId = request.RoleId;
        account.Status = string.IsNullOrWhiteSpace(request.Status) ? account.Status : request.Status.Trim();
        account.EmployeeId = request.EmployeeId;
        account.CustomerId = request.CustomerId;

        if (!string.IsNullOrWhiteSpace(request.Password))
        {
            account.Password = request.Password;
        }

        _unitOfWork.Accounts.Update(account);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var loaded = await _unitOfWork.Accounts.FirstOrDefaultAsync(
            new LegacyAccountByIdSpec(account.AccountId),
            cancellationToken);
        if (loaded == null)
        {
            throw new NotFoundException($"Account with ID {request.AccountId} not found");
        }

        return _mapper.Map<AccountInfoDto>(loaded);
    }
}
