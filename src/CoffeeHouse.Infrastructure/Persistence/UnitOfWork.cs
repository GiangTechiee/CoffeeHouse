using CoffeeHouse.Domain.Interfaces;
using CoffeeHouse.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace CoffeeHouse.Infrastructure.Persistence;

/// <summary>
/// Unit of Work implementation for coordinating multiple repositories and managing transactions
/// </summary>
public class UnitOfWork : IUnitOfWork
{
    private readonly CoffeeHouseContext _context;
    private IDbContextTransaction? _transaction;
    private bool _disposed;

    // Lazy-loaded repositories
    private IProductRepository? _products;
    private IOrderRepository? _orders;
    private ICustomerRepository? _customers;
    private IEmployeeRepository? _employees;
    private Domain.Interfaces.ICartRepository? _carts;
    private IProductCategoryRepository? _productCategories;
    private ICafeStoreRepository? _cafeStores;
    private IAccountRepository? _accounts;
    private IRoleRepository? _roles;
    private IRepository<Domain.Entities.RefreshToken>? _refreshTokens;

    /// <summary>
    /// Initializes a new instance of the UnitOfWork class
    /// </summary>
    /// <param name="context">Database context</param>
    /// <exception cref="ArgumentNullException">Thrown when context is null</exception>
    public UnitOfWork(CoffeeHouseContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    /// <summary>
    /// Gets the Product repository
    /// </summary>
    public IProductRepository Products
    {
        get
        {
            _products ??= new ProductRepository(_context);
            return _products;
        }
    }

    /// <summary>
    /// Gets the Order repository
    /// </summary>
    public IOrderRepository Orders
    {
        get
        {
            _orders ??= new OrderRepository(_context);
            return _orders;
        }
    }

    /// <summary>
    /// Gets the Customer repository
    /// </summary>
    public ICustomerRepository Customers
    {
        get
        {
            _customers ??= new CustomerRepository(_context);
            return _customers;
        }
    }

    /// <summary>
    /// Gets the Employee repository
    /// </summary>
    public IEmployeeRepository Employees
    {
        get
        {
            _employees ??= new EmployeeRepository(_context);
            return _employees;
        }
    }

    /// <summary>
    /// Gets the Cart repository
    /// </summary>
    public Domain.Interfaces.ICartRepository Carts
    {
        get
        {
            _carts ??= new Repositories.CartRepository(_context);
            return _carts;
        }
    }

    /// <summary>
    /// Gets the ProductCategory repository
    /// </summary>
    public IProductCategoryRepository ProductCategories
    {
        get
        {
            _productCategories ??= new ProductCategoryRepository(_context);
            return _productCategories;
        }
    }

    /// <summary>
    /// Gets the CafeStore repository
    /// </summary>
    public ICafeStoreRepository CafeStores
    {
        get
        {
            _cafeStores ??= new CafeStoreRepository(_context);
            return _cafeStores;
        }
    }

    /// <summary>
    /// Gets the Account repository
    /// </summary>
    public IAccountRepository Accounts
    {
        get
        {
            _accounts ??= new AccountRepository(_context);
            return _accounts;
        }
    }

    /// <summary>
    /// Gets the Role repository
    /// </summary>
    public IRoleRepository Roles
    {
        get
        {
            _roles ??= new RoleRepository(_context);
            return _roles;
        }
    }

    /// <summary>
    /// Gets the RefreshToken repository
    /// </summary>
    public IRepository<Domain.Entities.RefreshToken> RefreshTokens
    {
        get
        {
            _refreshTokens ??= new Repository<Domain.Entities.RefreshToken>(_context);
            return _refreshTokens;
        }
    }

    /// <summary>
    /// Saves all changes made in this unit of work to the database asynchronously
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The number of state entries written to the database</returns>
    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Begins a new database transaction asynchronously
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Task representing the asynchronous operation</returns>
    /// <exception cref="InvalidOperationException">Thrown when a transaction is already in progress</exception>
    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction != null)
        {
            throw new InvalidOperationException("A transaction is already in progress.");
        }

        _transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
    }

    /// <summary>
    /// Commits the current transaction asynchronously
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Task representing the asynchronous operation</returns>
    /// <exception cref="InvalidOperationException">Thrown when no transaction is in progress</exception>
    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction == null)
        {
            throw new InvalidOperationException("No transaction is in progress.");
        }

        try
        {
            await _context.SaveChangesAsync(cancellationToken);
            await _transaction.CommitAsync(cancellationToken);
        }
        catch
        {
            await RollbackTransactionAsync(cancellationToken);
            throw;
        }
        finally
        {
            _transaction?.Dispose();
            _transaction = null;
        }
    }

    /// <summary>
    /// Rolls back the current transaction asynchronously
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Task representing the asynchronous operation</returns>
    /// <exception cref="InvalidOperationException">Thrown when no transaction is in progress</exception>
    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction == null)
        {
            throw new InvalidOperationException("No transaction is in progress.");
        }

        try
        {
            await _transaction.RollbackAsync(cancellationToken);
        }
        finally
        {
            _transaction?.Dispose();
            _transaction = null;
        }
    }

    /// <summary>
    /// Disposes the Unit of Work and releases all resources
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Disposes the Unit of Work and releases resources
    /// </summary>
    /// <param name="disposing">True if disposing managed resources</param>
    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _transaction?.Dispose();
                _context?.Dispose();
            }

            _disposed = true;
        }
    }
}
