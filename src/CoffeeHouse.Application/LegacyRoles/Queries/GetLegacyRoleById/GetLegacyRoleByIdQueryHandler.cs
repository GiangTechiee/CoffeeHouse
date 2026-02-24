using CoffeeHouse.Application.LegacyRoles.DTOs;
using CoffeeHouse.Domain.Exceptions;
using CoffeeHouse.Domain.Interfaces;
using MediatR;

namespace CoffeeHouse.Application.LegacyRoles.Queries.GetLegacyRoleById;

public class GetLegacyRoleByIdQueryHandler : IRequestHandler<GetLegacyRoleByIdQuery, LegacyRoleDto>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetLegacyRoleByIdQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<LegacyRoleDto> Handle(GetLegacyRoleByIdQuery request, CancellationToken cancellationToken)
    {
        var role = await _unitOfWork.Roles.GetByIdAsync(request.RoleId, cancellationToken);
        if (role == null)
        {
            throw new NotFoundException($"Role with ID {request.RoleId} not found");
        }

        return new LegacyRoleDto
        {
            RoleId = role.RoleId,
            RoleName = role.RoleName,
            Description = role.Description
        };
    }
}
