using AutoMapper;
using CoffeeHouse.Application.Common.Models;
using CoffeeHouse.Application.Customers.DTOs;
using CoffeeHouse.Domain.Interfaces;
using MediatR;

namespace CoffeeHouse.Application.Customers.Queries.GetCustomers;

public record GetCustomersQuery(int PageNumber = 1, int PageSize = 10, string? SearchTerm = null) : IRequest<Result<PaginatedResponse<CustomerDto>>>
{
    public int PageNumber { get; init; } = PageNumber;
    public int PageSize { get; init; } = PageSize;
    public string? SearchTerm { get; init; } = SearchTerm;
}

public class GetCustomersQueryHandler : IRequestHandler<GetCustomersQuery, Result<PaginatedResponse<CustomerDto>>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetCustomersQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<PaginatedResponse<CustomerDto>>> Handle(GetCustomersQuery request, CancellationToken cancellationToken)
    {
        var (items, totalCount) = await _unitOfWork.Customers.GetPagedAsync(
            request.PageNumber, 
            request.PageSize, 
            request.SearchTerm, 
            cancellationToken);

        var dtos = _mapper.Map<List<CustomerDto>>(items);
        
        var paginatedList = new PaginatedResponse<CustomerDto>(
            dtos, 
            totalCount, 
            request.PageNumber, 
            request.PageSize);
            
        return Result<PaginatedResponse<CustomerDto>>.Success(paginatedList);
    }
}
