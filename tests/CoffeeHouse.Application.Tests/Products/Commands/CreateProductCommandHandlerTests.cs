using CoffeeHouse.Application.Products.Commands.CreateProduct;
using CoffeeHouse.Application.Products.DTOs;
using CoffeeHouse.Domain.Entities;
using CoffeeHouse.Domain.Interfaces;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace CoffeeHouse.Application.Tests.Products.Commands;

/// <summary>
/// Tests for CreateProductCommandHandler
/// Validates: Requirements 13.2, 13.4 - Use case handler logic and error scenarios
/// </summary>
public class CreateProductCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IProductRepository> _productRepositoryMock;
    private readonly Mock<ILogger<CreateProductCommandHandler>> _loggerMock;
    private readonly CreateProductCommandHandler _handler;

    public CreateProductCommandHandlerTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _productRepositoryMock = new Mock<IProductRepository>();
        _loggerMock = new Mock<ILogger<CreateProductCommandHandler>>();

        _unitOfWorkMock.Setup(u => u.Products).Returns(_productRepositoryMock.Object);

        _handler = new CreateProductCommandHandler(
            _unitOfWorkMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_WithValidCommand_ShouldCreateProductAndReturnDto()
    {
        // Arrange
        var command = new CreateProductCommand
        {
            Name = "Cappuccino",
            Price = 50000m,
            CategoryId = 1,
            Description = "Delicious coffee",
            ImageUrl = "/images/cappuccino.jpg",
            Notes = "Best seller"
        };

        _productRepositoryMock
            .Setup(r => r.GetByNameAsync(command.Name, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Product?)null);

        _productRepositoryMock
            .Setup(r => r.AddAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _unitOfWorkMock
            .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be(command.Name);
        result.Price.Should().Be(command.Price);
        result.CategoryId.Should().Be(command.CategoryId);
        result.Description.Should().Be(command.Description);
        result.ImageUrl.Should().Be(command.ImageUrl);
        result.Notes.Should().Be(command.Notes);

        _productRepositoryMock.Verify(
            r => r.GetByNameAsync(command.Name, It.IsAny<CancellationToken>()),
            Times.Once);

        _productRepositoryMock.Verify(
            r => r.AddAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()),
            Times.Once);

        _unitOfWorkMock.Verify(
            u => u.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_WhenProductNameAlreadyExists_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var command = new CreateProductCommand
        {
            Name = "Cappuccino",
            Price = 50000m,
            CategoryId = 1
        };

        var existingProduct = new Product { ProductName = "Cappuccino", Price = 45000m, CategoryId = 1 };

        _productRepositoryMock
            .Setup(r => r.GetByNameAsync(command.Name, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingProduct);

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*already exists*");

        _productRepositoryMock.Verify(
            r => r.GetByNameAsync(command.Name, It.IsAny<CancellationToken>()),
            Times.Once);

        _productRepositoryMock.Verify(
            r => r.AddAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()),
            Times.Never);

        _unitOfWorkMock.Verify(
            u => u.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_WithMinimalData_ShouldCreateProduct()
    {
        // Arrange
        var command = new CreateProductCommand
        {
            Name = "Espresso",
            Price = 35000m,
            CategoryId = 1
        };

        _productRepositoryMock
            .Setup(r => r.GetByNameAsync(command.Name, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Product?)null);

        _productRepositoryMock
            .Setup(r => r.AddAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _unitOfWorkMock
            .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be(command.Name);
        result.Price.Should().Be(command.Price);
        result.CategoryId.Should().Be(command.CategoryId);
        result.Description.Should().BeNull();
        result.ImageUrl.Should().BeNull();
        result.Notes.Should().BeNull();
    }

    [Fact]
    public async Task Handle_ShouldLogInformation()
    {
        // Arrange
        var command = new CreateProductCommand
        {
            Name = "Latte",
            Price = 45000m,
            CategoryId = 1
        };

        _productRepositoryMock
            .Setup(r => r.GetByNameAsync(command.Name, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Product?)null);

        _productRepositoryMock
            .Setup(r => r.AddAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _unitOfWorkMock
            .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Creating product")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);

        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Product created successfully")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_WhenProductNameExists_ShouldLogWarning()
    {
        // Arrange
        var command = new CreateProductCommand
        {
            Name = "Cappuccino",
            Price = 50000m,
            CategoryId = 1
        };

        var existingProduct = new Product("Cappuccino", 45000m, 1);

        _productRepositoryMock
            .Setup(r => r.GetByNameAsync(command.Name, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingProduct);

        // Act
        try
        {
            await _handler.Handle(command, CancellationToken.None);
        }
        catch (InvalidOperationException)
        {
            // Expected exception
        }

        // Assert
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("already exists")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }
}
