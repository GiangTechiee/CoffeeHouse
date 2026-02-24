using AutoMapper;
using CoffeeHouse.Application.Authentication.DTOs;
using CoffeeHouse.Application.LegacyAccounts.Specifications;
using CoffeeHouse.Domain.Exceptions;
using CoffeeHouse.Domain.Interfaces;
using MediatR;

namespace CoffeeHouse.Application.LegacyAccounts.Queries.GetLegacyAccountById;

public class GetLegacyAccountByIdQueryHandler : IRequestHandler<GetLegacyAccountByIdQuery, AccountInfoDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetLegacyAccountByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<AccountInfoDto> Handle(GetLegacyAccountByIdQuery request, CancellationToken cancellationToken)
    {
        var spec = new LegacyAccountByIdSpec(request.AccountId);
        var account = await _unitOfWork.Accounts.FirstOrDefaultAsync(spec, cancellationToken);
        if (account == null)
        {
            throw new NotFoundException($"Account with ID {request.AccountId} not found");
        }

        return _mapper.Map<AccountInfoDto>(account);
    }
}
