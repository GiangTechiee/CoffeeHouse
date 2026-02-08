using CoffeeHouse.Domain.Entities;

namespace CoffeeHouse.Domain.Interfaces;

/// <summary>
/// Repository interface for cart operations
/// </summary>
public interface ICartRepository
{
    /// <summary>
    /// Gets a cart item for a specific customer and product
    /// </summary>
    /// <param name="customerId">Customer ID</param>
    /// <param name="productId">Product ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Cart item if found, null otherwise</returns>
    Task<CartItem?> GetCartItemAsync(int customerId, int productId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all cart items for a specific customer
    /// </summary>
    /// <param name="customerId">Customer ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of cart items</returns>
    Task<List<CartItem>> GetCartItemsAsync(int customerId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a new cart item
    /// </summary>
    /// <param name="cartItem">Cart item to add</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task AddAsync(CartItem cartItem, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing cart item
    /// </summary>
    /// <param name="cartItem">Cart item to update</param>
    void Update(CartItem cartItem);

    /// <summary>
    /// Removes a cart item
    /// </summary>
    /// <param name="cartItem">Cart item to remove</param>
    void Remove(CartItem cartItem);

    /// <summary>
    /// Removes all cart items for a specific customer
    /// </summary>
    /// <param name="customerId">Customer ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task RemoveAllAsync(int customerId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the total count of items in a customer's cart
    /// </summary>
    /// <param name="customerId">Customer ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Total count of items</returns>
    Task<int> GetCartCountAsync(int customerId, CancellationToken cancellationToken = default);
}
