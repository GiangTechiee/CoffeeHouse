using MediatR;

namespace CoffeeHouse.Application.LegacyAccounts.Commands.DeleteLegacyAccount;

public record DeleteLegacyAccountCommand(int AccountId) : IRequest;

