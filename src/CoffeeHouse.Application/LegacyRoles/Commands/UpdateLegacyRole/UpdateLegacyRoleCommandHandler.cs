using CoffeeHouse.Application.LegacyRoles.DTOs;
using CoffeeHouse.Domain.Exceptions;
using CoffeeHouse.Domain.Interfaces;
using MediatR;

namespace CoffeeHouse.Application.LegacyRoles.Commands.UpdateLegacyRole;

public class UpdateLegacyRoleCommandHandler : IRequestHandler<UpdateLegacyRoleCommand, LegacyRoleDto>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateLegacyRoleCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<LegacyRoleDto> Handle(UpdateLegacyRoleCommand request, CancellationToken cancellationToken)
    {
        var role = await _unitOfWork.Roles.GetByIdAsync(request.RoleId, cancellationToken);
        if (role == null)
        {
            throw new NotFoundException($"Role with ID {request.RoleId} not found");
        }

        if (!string.Equals(role.RoleName, request.Name, StringComparison.OrdinalIgnoreCase))
        {
            var exists = await _unitOfWork.Roles.GetByNameAsync(request.Name, cancellationToken);
            if (exists != null)
            {
                throw new InvalidOperationException($"Role {request.Name} already exists");
            }
        }

        role.RoleName = request.Name.Trim();
        role.Description = string.IsNullOrWhiteSpace(request.Description) ? null : request.Description.Trim();

        _unitOfWork.Roles.Update(role);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new LegacyRoleDto
        {
            RoleId = role.RoleId,
            RoleName = role.RoleName,
            Description = role.Description
        };
    }
}
