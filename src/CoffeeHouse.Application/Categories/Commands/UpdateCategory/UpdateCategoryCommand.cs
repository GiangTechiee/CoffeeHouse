
using CoffeeHouse.Application.Categories.DTOs;
using CoffeeHouse.Domain.Interfaces;
using CoffeeHouse.Domain.Exceptions;
using MediatR;
using AutoMapper;

namespace CoffeeHouse.Application.Categories.Commands.UpdateCategory;

public record UpdateCategoryCommand : IRequest<CategoryDto>
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
}

public class UpdateCategoryCommandHandler : IRequestHandler<UpdateCategoryCommand, CategoryDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public UpdateCategoryCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<CategoryDto> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await _unitOfWork.ProductCategories.GetByIdAsync(request.Id);

        if (category == null)
        {
            throw new NotFoundException($"ProductCategory with ID {request.Id} not found");
        }

        category.CategoryName = request.Name;
        // Description not in Entity

        _unitOfWork.ProductCategories.Update(category);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<CategoryDto>(category);
    }
}
