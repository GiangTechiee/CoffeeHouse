using CoffeeHouse.Application.Common.Exceptions;
using CoffeeHouse.Domain.Exceptions;
using CoffeeHouse.Domain.Identity;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace CoffeeHouse.Application.Roles.Commands.DeleteRole;

public record DeleteRoleCommand(int Id) : IRequest;

public class DeleteRoleCommandHandler : IRequestHandler<DeleteRoleCommand>
{
    private readonly RoleManager<AppRole> _roleManager;

    public DeleteRoleCommandHandler(RoleManager<AppRole> roleManager)
    {
        _roleManager = roleManager;
    }

    public async Task Handle(DeleteRoleCommand request, CancellationToken cancellationToken)
    {
        var role = await _roleManager.FindByIdAsync(request.Id.ToString());
        if (role == null)
        {
            throw new NotFoundException($"Role with ID {request.Id} not found");
        }

        var result = await _roleManager.DeleteAsync(role);
        if (!result.Succeeded)
        {
            throw new ApiOperationException("DELETE_ROLE_FAILED", "Failed to delete role", result.Errors);
        }
    }
}
