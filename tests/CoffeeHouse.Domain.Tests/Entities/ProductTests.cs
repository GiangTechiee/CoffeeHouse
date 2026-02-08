using CoffeeHouse.Domain.Entities;
using CoffeeHouse.Domain.Exceptions;
using FluentAssertions;
using Xunit;

namespace CoffeeHouse.Domain.Tests.Entities;

/// <summary>
/// Tests for Product entity
/// Validates: Requirements 13.2 - Domain entity logic and validation
/// </summary>
public class ProductTests
{
    [Fact]
    public void Constructor_WithValidData_ShouldCreateProduct()
    {
        // Arrange
        var name = "Cappuccino";
        var price = 50000m;
        var categoryId = 1;
        var description = "Delicious coffee";

        // Act
        var product = new Product
        {
            ProductName = name,
            Price = price,
            CategoryId = categoryId,
            Description = description
        };

        // Assert
        product.ProductName.Should().Be(name);
        product.Price.Should().Be(price);
        product.CategoryId.Should().Be(categoryId);
        product.Description.Should().Be(description);
        product.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void UpdatePrice_WithValidPrice_ShouldUpdatePrice()
    {
        // Arrange
        var product = new Product
        {
            ProductName = "Cappuccino",
            Price = 50000m,
            CategoryId = 1
        };
        var newPrice = 60000m;

        // Act
        product.Price = newPrice;

        // Assert
        product.Price.Should().Be(newPrice);
    }
}
