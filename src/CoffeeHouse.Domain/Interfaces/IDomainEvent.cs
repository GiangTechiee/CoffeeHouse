using MediatR;

namespace CoffeeHouse.Domain.Interfaces;

/// <summary>
/// Interface for Domain Events
/// </summary>
public interface IDomainEvent : INotification
{
    DateTime OccurredOn { get; }
}
