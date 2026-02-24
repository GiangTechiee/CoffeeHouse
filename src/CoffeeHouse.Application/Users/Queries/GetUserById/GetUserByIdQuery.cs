using CoffeeHouse.Application.Users.DTOs;
using MediatR;

namespace CoffeeHouse.Application.Users.Queries.GetUserById;

public record GetUserByIdQuery(int Id) : IRequest<UserDto?>;
