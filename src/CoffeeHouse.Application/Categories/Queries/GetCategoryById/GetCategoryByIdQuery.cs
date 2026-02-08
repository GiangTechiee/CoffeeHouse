using AutoMapper;
using CoffeeHouse.Application.Categories.DTOs;
using CoffeeHouse.Domain.Interfaces;
using CoffeeHouse.Domain.Exceptions;
using MediatR;

namespace CoffeeHouse.Application.Categories.Queries.GetCategoryById;

public record GetCategoryByIdQuery : IRequest<CategoryDto>
{
    public int Id { get; set; }
}

public class GetCategoryByIdQueryHandler : IRequestHandler<GetCategoryByIdQuery, CategoryDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetCategoryByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<CategoryDto> Handle(GetCategoryByIdQuery request, CancellationToken cancellationToken)
    {
        var category = await _unitOfWork.ProductCategories.GetByIdAsync(request.Id);

        if (category == null)
        {
            throw new NotFoundException($"ProductCategory with ID {request.Id} not found");
        }

        return _mapper.Map<CategoryDto>(category);
    }
}
