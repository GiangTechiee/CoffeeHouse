using CoffeeHouse.Application.Common.Exceptions;
using CoffeeHouse.Domain.Exceptions;
using CoffeeHouse.Domain.Identity;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace CoffeeHouse.Application.Users.Commands.DeleteUser;

public record DeleteUserCommand(int Id) : IRequest;

public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand>
{
    private readonly UserManager<AppUser> _userManager;

    public DeleteUserCommandHandler(UserManager<AppUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.Id.ToString());
        if (user == null)
        {
            throw new NotFoundException($"User with ID {request.Id} not found");
        }

        var result = await _userManager.DeleteAsync(user);
        if (!result.Succeeded)
        {
            throw new ApiOperationException("DELETE_USER_FAILED", "Failed to delete user", result.Errors);
        }
    }
}
