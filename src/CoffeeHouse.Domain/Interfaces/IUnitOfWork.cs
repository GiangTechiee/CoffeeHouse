namespace CoffeeHouse.Domain.Interfaces;

/// <summary>
/// Unit of Work interface for coordinating multiple repositories and managing transactions
/// </summary>
public interface IUnitOfWork : IDisposable
{
    /// <summary>
    /// Gets the Product repository
    /// </summary>
    IProductRepository Products { get; }

    /// <summary>
    /// Gets the Order repository
    /// </summary>
    IOrderRepository Orders { get; }

    /// <summary>
    /// Gets the Customer repository
    /// </summary>
    ICustomerRepository Customers { get; }

    /// <summary>
    /// Gets the Employee repository
    /// </summary>
    IEmployeeRepository Employees { get; }

    /// <summary>
    /// Gets the Cart repository (temporary during migration)
    /// </summary>
    ICartRepository Carts { get; }

    /// <summary>
    /// Gets the ProductCategory repository
    /// </summary>
    IProductCategoryRepository ProductCategories { get; }

    /// <summary>
    /// Gets the CafeStore repository
    /// </summary>
    ICafeStoreRepository CafeStores { get; }

    /// <summary>
    /// Gets the Account repository
    /// </summary>
    IAccountRepository Accounts { get; }

    /// <summary>
    /// Gets the Role repository
    /// </summary>
    IRoleRepository Roles { get; }

    /// <summary>
    /// Gets the RefreshToken repository
    /// </summary>
    IRepository<CoffeeHouse.Domain.Entities.RefreshToken> RefreshTokens { get; }

    /// <summary>
    /// Gets the Supplier repository
    /// </summary>
    ISupplierRepository Suppliers { get; }

    /// <summary>
    /// Gets the Ingredient repository
    /// </summary>
    IIngredientRepository Ingredients { get; }

    /// <summary>
    /// Gets the News repository
    /// </summary>
    INewsRepository NewsArticles { get; }

    /// <summary>
    /// Gets the PurchaseOrder repository
    /// </summary>
    IPurchaseOrderRepository PurchaseOrders { get; }

    /// <summary>
    /// Saves all changes made in this unit of work to the database asynchronously
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The number of state entries written to the database</returns>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Begins a new database transaction asynchronously
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Task representing the asynchronous operation</returns>
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Commits the current transaction asynchronously
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Task representing the asynchronous operation</returns>
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Rolls back the current transaction asynchronously
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Task representing the asynchronous operation</returns>
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
}
