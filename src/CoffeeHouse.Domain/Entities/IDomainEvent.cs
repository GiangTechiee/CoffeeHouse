namespace CoffeeHouse.Domain.Entities;

/// <summary>
/// Marker interface for domain events
/// </summary>
public interface IDomainEvent
{
    /// <summary>
    /// Timestamp when the event occurred
    /// </summary>
    DateTime OccurredOn { get; }
}
