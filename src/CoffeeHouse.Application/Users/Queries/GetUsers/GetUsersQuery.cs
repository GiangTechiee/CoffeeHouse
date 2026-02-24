using CoffeeHouse.Application.Common.Models;
using CoffeeHouse.Application.Users.DTOs;
using MediatR;

namespace CoffeeHouse.Application.Users.Queries.GetUsers;

public record GetUsersQuery(int PageNumber = 1, int PageSize = 20, string? SearchTerm = null)
    : IRequest<PagedResult<UserDto>>;
