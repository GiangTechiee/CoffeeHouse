using CoffeeHouse.Application.LegacyRoles.DTOs;
using CoffeeHouse.Domain.Entities;
using CoffeeHouse.Domain.Interfaces;
using MediatR;

namespace CoffeeHouse.Application.LegacyRoles.Commands.CreateLegacyRole;

public class CreateLegacyRoleCommandHandler : IRequestHandler<CreateLegacyRoleCommand, LegacyRoleDto>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateLegacyRoleCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<LegacyRoleDto> Handle(CreateLegacyRoleCommand request, CancellationToken cancellationToken)
    {
        var exists = await _unitOfWork.Roles.GetByNameAsync(request.Name, cancellationToken);
        if (exists != null)
        {
            throw new InvalidOperationException($"Role {request.Name} already exists");
        }

        var role = new Role
        {
            RoleName = request.Name.Trim(),
            Description = string.IsNullOrWhiteSpace(request.Description) ? null : request.Description.Trim()
        };

        await _unitOfWork.Roles.AddAsync(role, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new LegacyRoleDto
        {
            RoleId = role.RoleId,
            RoleName = role.RoleName,
            Description = role.Description
        };
    }
}
