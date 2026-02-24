using CoffeeHouse.Application.LegacyRoles.DTOs;
using CoffeeHouse.Domain.Interfaces;
using MediatR;

namespace CoffeeHouse.Application.LegacyRoles.Queries.GetLegacyRoles;

public class GetLegacyRolesQueryHandler : IRequestHandler<GetLegacyRolesQuery, List<LegacyRoleDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetLegacyRolesQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<List<LegacyRoleDto>> Handle(GetLegacyRolesQuery request, CancellationToken cancellationToken)
    {
        var roles = await _unitOfWork.Roles.GetAllAsync(cancellationToken);

        return roles.Select(r => new LegacyRoleDto
        {
            RoleId = r.RoleId,
            RoleName = r.RoleName,
            Description = r.Description
        }).ToList();
    }
}
