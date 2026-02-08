using CoffeeHouse.Domain.Interfaces;
using CoffeeHouse.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CoffeeHouse.Infrastructure.Persistence.Repositories;

/// <summary>
/// Repository implementation for cart operations
/// </summary>
public class CartRepository : ICartRepository
{
    private readonly CoffeeHouseContext _context;

    /// <summary>
    /// Initializes a new instance of CartRepository
    /// </summary>
    /// <param name="context">Database context</param>
    public CartRepository(CoffeeHouseContext context)
    {
        _context = context;
    }

    /// <inheritdoc/>
    public async Task<CartItem?> GetCartItemAsync(int customerId, int productId, CancellationToken cancellationToken = default)
    {
        return await _context.CartItems
            .Include(g => g.Product)
            .FirstOrDefaultAsync(g => g.CustomerId == customerId && g.ProductId == productId, cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<List<CartItem>> GetCartItemsAsync(int customerId, CancellationToken cancellationToken = default)
    {
        return await _context.CartItems
            .Include(g => g.Product)
            .Where(g => g.CustomerId == customerId)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc/>
    public async Task AddAsync(CartItem cartItem, CancellationToken cancellationToken = default)
    {
        await _context.CartItems.AddAsync(cartItem, cancellationToken);
    }

    /// <inheritdoc/>
    public void Update(CartItem cartItem)
    {
        _context.CartItems.Update(cartItem);
    }

    /// <inheritdoc/>
    public void Remove(CartItem cartItem)
    {
        _context.CartItems.Remove(cartItem);
    }

    /// <inheritdoc/>
    public async Task RemoveAllAsync(int customerId, CancellationToken cancellationToken = default)
    {
        var cartItems = await _context.CartItems
            .Where(c => c.CustomerId == customerId)
            .ToListAsync(cancellationToken);
        
        _context.CartItems.RemoveRange(cartItems);
    }

    /// <inheritdoc/>
    public async Task<int> GetCartCountAsync(int customerId, CancellationToken cancellationToken = default)
    {
        var cartItems = await _context.CartItems
            .Where(g => g.CustomerId == customerId)
            .ToListAsync(cancellationToken);

        return cartItems.Sum(item => item.Quantity);
    }
}
