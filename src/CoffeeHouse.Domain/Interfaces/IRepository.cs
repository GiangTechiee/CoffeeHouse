namespace CoffeeHouse.Domain.Interfaces;

/// <summary>
/// Generic repository interface for common CRUD operations
/// </summary>
/// <typeparam name="T">Entity type that inherits from BaseEntity</typeparam>
public interface IRepository<T> where T : class
{
    /// <summary>
    /// Gets an entity by its ID asynchronously
    /// </summary>
    /// <param name="id">Entity ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Entity if found, null otherwise</returns>
    Task<T?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all entities asynchronously
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Read-only list of all entities</returns>
    Task<IReadOnlyList<T>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a new entity asynchronously
    /// </summary>
    /// <param name="entity">Entity to add</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Task representing the asynchronous operation</returns>
    Task AddAsync(T entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing entity
    /// </summary>
    /// <param name="entity">Entity to update</param>
    /// <remarks>
    /// This method marks the entity as modified. Changes are persisted when SaveChanges is called on the Unit of Work.
    /// </remarks>
    void Update(T entity);

    /// <summary>
    /// Deletes an entity
    /// </summary>
    /// <param name="entity">Entity to delete</param>
    void Delete(T entity);

    /// <summary>
    /// List entities with specification
    /// </summary>
    Task<IReadOnlyList<T>> ListAsync(CoffeeHouse.Domain.Specifications.ISpecification<T> spec, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get first entity with specification
    /// </summary>
    Task<T?> FirstOrDefaultAsync(CoffeeHouse.Domain.Specifications.ISpecification<T> spec, CancellationToken cancellationToken = default);

    /// <summary>
    /// Count entities with specification
    /// </summary>
    Task<int> CountAsync(CoffeeHouse.Domain.Specifications.ISpecification<T> spec, CancellationToken cancellationToken = default);
}
