using AutoMapper;
using CoffeeHouse.Application.Authentication.DTOs;
using CoffeeHouse.Application.Common.Models;
using CoffeeHouse.Application.LegacyAccounts.Specifications;
using CoffeeHouse.Domain.Interfaces;
using MediatR;

namespace CoffeeHouse.Application.LegacyAccounts.Queries.GetLegacyAccounts;

public class GetLegacyAccountsQueryHandler : IRequestHandler<GetLegacyAccountsQuery, PagedResult<AccountInfoDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetLegacyAccountsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<PagedResult<AccountInfoDto>> Handle(GetLegacyAccountsQuery request, CancellationToken cancellationToken)
    {
        var pageNumber = request.PageNumber < 1 ? 1 : request.PageNumber;
        var pageSize = request.PageSize < 1 ? 20 : request.PageSize;
        if (pageSize > 100) pageSize = 100;

        var skip = (pageNumber - 1) * pageSize;
        var listSpec = new LegacyAccountsSpec(request.SearchTerm, skip, pageSize);
        var countSpec = new LegacyAccountsCountSpec(request.SearchTerm);

        var items = await _unitOfWork.Accounts.ListAsync(listSpec, cancellationToken);
        var totalCount = await _unitOfWork.Accounts.CountAsync(countSpec, cancellationToken);

        var dtos = items.Select(a => _mapper.Map<AccountInfoDto>(a)).ToList();

        return new PagedResult<AccountInfoDto>
        {
            Items = dtos,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalCount = totalCount
        };
    }
}
