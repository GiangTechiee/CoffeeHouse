using AutoMapper;
using CoffeeHouse.Application.Authentication.DTOs;
using CoffeeHouse.Domain.Interfaces;
using MediatR;

namespace CoffeeHouse.Application.Authentication.Queries.GetAccountInfo;

/// <summary>
/// Handler for GetAccountInfoQuery
/// </summary>
public class GetAccountInfoQueryHandler : IRequestHandler<GetAccountInfoQuery, AccountInfoDto?>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAccountInfoQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<AccountInfoDto?> Handle(GetAccountInfoQuery request, CancellationToken cancellationToken)
    {
        var account = await _unitOfWork.Accounts.GetByUsernameAsync(request.Username, cancellationToken);

        if (account == null)
        {
            return null;
        }

        return _mapper.Map<AccountInfoDto>(account);
    }
}
