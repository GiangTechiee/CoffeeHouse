using CoffeeHouse.Application.Products.Queries.GetProducts;
using CoffeeHouse.Domain.Entities;
using CoffeeHouse.Domain.Interfaces;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace CoffeeHouse.Application.Tests.Products.Queries;

/// <summary>
/// Tests for GetProductsQueryHandler
/// Validates: Requirements 13.2, 13.4 - Query handler logic and pagination
/// </summary>
public class GetProductsQueryHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IProductRepository> _productRepositoryMock;
    private readonly Mock<ILogger<GetProductsQueryHandler>> _loggerMock;
    private readonly GetProductsQueryHandler _handler;

    public GetProductsQueryHandlerTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _productRepositoryMock = new Mock<IProductRepository>();
        _loggerMock = new Mock<ILogger<GetProductsQueryHandler>>();

        _unitOfWorkMock.Setup(u => u.Products).Returns(_productRepositoryMock.Object);

        _handler = new GetProductsQueryHandler(
            _unitOfWorkMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_WithValidQuery_ShouldReturnPagedProducts()
    {
        // Arrange
        var query = new GetProductsQuery
        {
            PageNumber = 1,
            PageSize = 10
        };

        var products = new List<Product>
        {
            new Product("Cappuccino", 50000m, 1),
            new Product("Latte", 45000m, 1),
            new Product("Espresso", 35000m, 1)
        };

        _productRepositoryMock
            .Setup(r => r.GetPagedAsync(
                It.IsAny<int?>(),
                It.IsAny<string?>(),
                It.IsAny<decimal?>(),
                It.IsAny<decimal?>(),
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((products, 3));

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Items.Should().HaveCount(3);
        result.PageNumber.Should().Be(1);
        result.PageSize.Should().Be(10);
        result.TotalCount.Should().Be(3);
        result.TotalPages.Should().Be(1);

        _productRepositoryMock.Verify(
            r => r.GetPagedAsync(
                It.IsAny<int?>(),
                It.IsAny<string?>(),
                It.IsAny<decimal?>(),
                It.IsAny<decimal?>(),
                1,
                10,
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_WithCategoryFilter_ShouldPassCategoryIdToRepository()
    {
        // Arrange
        var query = new GetProductsQuery
        {
            CategoryId = 2,
            PageNumber = 1,
            PageSize = 10
        };

        var products = new List<Product>
        {
            new Product("Mocha", 55000m, 2)
        };

        _productRepositoryMock
            .Setup(r => r.GetPagedAsync(
                2,
                It.IsAny<string?>(),
                It.IsAny<decimal?>(),
                It.IsAny<decimal?>(),
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((products, 1));

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Items.Should().HaveCount(1);
        result.Items.First().CategoryId.Should().Be(2);

        _productRepositoryMock.Verify(
            r => r.GetPagedAsync(
                2,
                It.IsAny<string?>(),
                It.IsAny<decimal?>(),
                It.IsAny<decimal?>(),
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_WithSearchTerm_ShouldPassSearchTermToRepository()
    {
        // Arrange
        var query = new GetProductsQuery
        {
            SearchTerm = "coffee",
            PageNumber = 1,
            PageSize = 10
        };

        var products = new List<Product>
        {
            new Product("Coffee Latte", 45000m, 1)
        };

        _productRepositoryMock
            .Setup(r => r.GetPagedAsync(
                It.IsAny<int?>(),
                "coffee",
                It.IsAny<decimal?>(),
                It.IsAny<decimal?>(),
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((products, 1));

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Items.Should().HaveCount(1);

        _productRepositoryMock.Verify(
            r => r.GetPagedAsync(
                It.IsAny<int?>(),
                "coffee",
                It.IsAny<decimal?>(),
                It.IsAny<decimal?>(),
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_WithPriceRange_ShouldPassPriceRangeToRepository()
    {
        // Arrange
        var query = new GetProductsQuery
        {
            MinPrice = 40000m,
            MaxPrice = 60000m,
            PageNumber = 1,
            PageSize = 10
        };

        var products = new List<Product>
        {
            new Product("Cappuccino", 50000m, 1),
            new Product("Latte", 45000m, 1)
        };

        _productRepositoryMock
            .Setup(r => r.GetPagedAsync(
                It.IsAny<int?>(),
                It.IsAny<string?>(),
                40000m,
                60000m,
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((products, 2));

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Items.Should().HaveCount(2);

        _productRepositoryMock.Verify(
            r => r.GetPagedAsync(
                It.IsAny<int?>(),
                It.IsAny<string?>(),
                40000m,
                60000m,
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Theory]
    [InlineData(0, 1)]
    [InlineData(-1, 1)]
    public async Task Handle_WithInvalidPageNumber_ShouldDefaultToOne(int invalidPageNumber, int expectedPageNumber)
    {
        // Arrange
        var query = new GetProductsQuery
        {
            PageNumber = invalidPageNumber,
            PageSize = 10
        };

        var products = new List<Product>();

        _productRepositoryMock
            .Setup(r => r.GetPagedAsync(
                It.IsAny<int?>(),
                It.IsAny<string?>(),
                It.IsAny<decimal?>(),
                It.IsAny<decimal?>(),
                expectedPageNumber,
                It.IsAny<int>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((products, 0));

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.PageNumber.Should().Be(expectedPageNumber);

        _productRepositoryMock.Verify(
            r => r.GetPagedAsync(
                It.IsAny<int?>(),
                It.IsAny<string?>(),
                It.IsAny<decimal?>(),
                It.IsAny<decimal?>(),
                expectedPageNumber,
                It.IsAny<int>(),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Theory]
    [InlineData(0, 10)]
    [InlineData(-1, 10)]
    public async Task Handle_WithInvalidPageSize_ShouldDefaultToTen(int invalidPageSize, int expectedPageSize)
    {
        // Arrange
        var query = new GetProductsQuery
        {
            PageNumber = 1,
            PageSize = invalidPageSize
        };

        var products = new List<Product>();

        _productRepositoryMock
            .Setup(r => r.GetPagedAsync(
                It.IsAny<int?>(),
                It.IsAny<string?>(),
                It.IsAny<decimal?>(),
                It.IsAny<decimal?>(),
                It.IsAny<int>(),
                expectedPageSize,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((products, 0));

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.PageSize.Should().Be(expectedPageSize);

        _productRepositoryMock.Verify(
            r => r.GetPagedAsync(
                It.IsAny<int?>(),
                It.IsAny<string?>(),
                It.IsAny<decimal?>(),
                It.IsAny<decimal?>(),
                It.IsAny<int>(),
                expectedPageSize,
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_WithPageSizeExceedingMaximum_ShouldLimitToOneHundred()
    {
        // Arrange
        var query = new GetProductsQuery
        {
            PageNumber = 1,
            PageSize = 200
        };

        var products = new List<Product>();

        _productRepositoryMock
            .Setup(r => r.GetPagedAsync(
                It.IsAny<int?>(),
                It.IsAny<string?>(),
                It.IsAny<decimal?>(),
                It.IsAny<decimal?>(),
                It.IsAny<int>(),
                100,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((products, 0));

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.PageSize.Should().Be(100);

        _productRepositoryMock.Verify(
            r => r.GetPagedAsync(
                It.IsAny<int?>(),
                It.IsAny<string?>(),
                It.IsAny<decimal?>(),
                It.IsAny<decimal?>(),
                It.IsAny<int>(),
                100,
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_WhenNoProductsFound_ShouldReturnEmptyResult()
    {
        // Arrange
        var query = new GetProductsQuery
        {
            PageNumber = 1,
            PageSize = 10
        };

        var products = new List<Product>();

        _productRepositoryMock
            .Setup(r => r.GetPagedAsync(
                It.IsAny<int?>(),
                It.IsAny<string?>(),
                It.IsAny<decimal?>(),
                It.IsAny<decimal?>(),
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((products, 0));

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Items.Should().BeEmpty();
        result.TotalCount.Should().Be(0);
        result.TotalPages.Should().Be(0);
    }

    [Fact]
    public async Task Handle_ShouldLogInformation()
    {
        // Arrange
        var query = new GetProductsQuery
        {
            PageNumber = 1,
            PageSize = 10
        };

        var products = new List<Product>
        {
            new Product("Cappuccino", 50000m, 1)
        };

        _productRepositoryMock
            .Setup(r => r.GetPagedAsync(
                It.IsAny<int?>(),
                It.IsAny<string?>(),
                It.IsAny<decimal?>(),
                It.IsAny<decimal?>(),
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((products, 1));

        // Act
        await _handler.Handle(query, CancellationToken.None);

        // Assert
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Getting products")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);

        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Retrieved")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }
}
