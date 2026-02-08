using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using CoffeeHouse.Domain.Interfaces;

namespace CoffeeHouse.Domain.Entities
{
    /// <summary>
    /// Base class for all domain entities to ensure consistency in tracking and auditing.
    /// </summary>
    public abstract class BaseEntity
    {
        private readonly List<IDomainEvent> _domainEvents = new();

        [NotMapped]
        public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

        public void AddDomainEvent(IDomainEvent domainEvent) => _domainEvents.Add(domainEvent);
        public void RemoveDomainEvent(IDomainEvent domainEvent) => _domainEvents.Remove(domainEvent);
        public void ClearDomainEvents() => _domainEvents.Clear();

        /// <summary>
        /// Audit field: When the record was created
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Audit field: Who created the record
        /// </summary>
        public string? CreatedBy { get; set; }

        /// <summary>
        /// Audit field: When the record was last updated
        /// </summary>
        public DateTime? UpdatedAt { get; set; }

        /// <summary>
        /// Audit field: Who last updated the record
        /// </summary>
        public string? UpdatedBy { get; set; }

        /// <summary>
        /// Support for Soft Delete: If true, the record is considered deleted but remains in the database for auditing.
        /// </summary>
        public bool IsDeleted { get; set; } = false;

        /// <summary>
        /// Optimistic Concurrency control
        /// </summary>
        public uint RowVersion { get; set; }
    }

    /// <summary>
    /// Base class for entities with a specific primary key type.
    /// </summary>
    public abstract class BaseEntity<TKey> : BaseEntity
    {
        public TKey Id { get; set; }
    }
}
