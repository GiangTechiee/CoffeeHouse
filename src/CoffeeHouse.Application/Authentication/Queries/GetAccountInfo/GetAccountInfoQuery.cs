using CoffeeHouse.Application.Authentication.DTOs;
using MediatR;

namespace CoffeeHouse.Application.Authentication.Queries.GetAccountInfo;

/// <summary>
/// Query for getting detailed account information
/// </summary>
public record GetAccountInfoQuery(string Username) : IRequest<AccountInfoDto?>;
