using CoffeeHouse.Domain.Entities;
using FluentAssertions;
using Xunit;

namespace CoffeeHouse.Domain.Tests.Entities;

/// <summary>
/// Tests for SalesOrder entity
/// </summary>
public class SalesOrderTests
{
    [Fact]
    public void Constructor_ShouldInitializeWithPendingStatus()
    {
        // Act
        var order = new SalesOrder();

        // Assert
        order.Status.Should().Be("Pending");
        order.SalesOrderItems.Should().BeEmpty();
        order.TotalAmount.Should().Be(0);
    }

    [Fact]
    public void AddItem_WhenPending_ShouldAddItemAndCalculateTotal()
    {
        // Arrange
        var order = new SalesOrder { Status = "Pending" };
        var productId = 1;
        var quantity = 2;
        var price = 50000m;

        // Act
        order.AddItem(productId, quantity, price);

        // Assert
        order.SalesOrderItems.Should().HaveCount(1);
        order.TotalAmount.Should().Be(quantity * price);
    }

    [Fact]
    public void AddItem_WhenNotPending_ShouldThrowException()
    {
        // Arrange
        var order = new SalesOrder { Status = "Confirmed" };

        // Act
        Action act = () => order.AddItem(1, 1, 10000m);

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("Cannot add items to an order that is not pending");
    }

    [Fact]
    public void Confirm_WhenHasItems_ShouldUpdateStatus()
    {
        // Arrange
        var order = new SalesOrder { Status = "Pending" };
        order.AddItem(1, 1, 10000m);

        // Act
        order.Confirm();

        // Assert
        order.Status.Should().Be("Confirmed");
    }

    [Fact]
    public void Confirm_WhenEmpty_ShouldThrowException()
    {
        // Arrange
        var order = new SalesOrder { Status = "Pending" };

        // Act
        Action act = () => order.Confirm();

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("Cannot confirm an order with no items");
    }
}
