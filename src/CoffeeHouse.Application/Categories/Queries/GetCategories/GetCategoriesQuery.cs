using AutoMapper;
using CoffeeHouse.Application.Categories.DTOs;
using CoffeeHouse.Application.Common.Models;
using CoffeeHouse.Domain.Interfaces;
using MediatR;

namespace CoffeeHouse.Application.Categories.Queries.GetCategories;

public record GetCategoriesQuery : IRequest<PagedResult<CategoryDto>>
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SearchTerm { get; set; }
}

public class GetCategoriesQueryHandler : IRequestHandler<GetCategoriesQuery, PagedResult<CategoryDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetCategoriesQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<PagedResult<CategoryDto>> Handle(GetCategoriesQuery request, CancellationToken cancellationToken)
    {
        var allCategories = await _unitOfWork.ProductCategories.GetAllAsync(cancellationToken);
        
        if (!string.IsNullOrEmpty(request.SearchTerm))
        {
            allCategories = allCategories
                .Where(c => c.CategoryName.Contains(request.SearchTerm, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        var totalCount = allCategories.Count;
        var items = allCategories
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToList();

        var dtos = _mapper.Map<List<CategoryDto>>(items);

        return new PagedResult<CategoryDto>
        {
            Items = dtos,
            TotalCount = totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };
    }
}
