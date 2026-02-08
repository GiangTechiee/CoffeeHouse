using CoffeeHouse.Domain.Exceptions;
using CoffeeHouse.Domain.ValueObjects;
using FluentAssertions;
using Xunit;

namespace CoffeeHouse.Domain.Tests.ValueObjects;

/// <summary>
/// Tests for Money value object
/// Validates: Requirements 13.2 - Value object equality and operations
/// </summary>
public class MoneyTests
{
    [Fact]
    public void Constructor_WithValidData_ShouldCreateMoney()
    {
        // Arrange
        var amount = 100m;
        var currency = "VND";

        // Act
        var money = new Money(amount, currency);

        // Assert
        money.Amount.Should().Be(amount);
        money.Currency.Should().Be(currency);
    }

    [Fact]
    public void Constructor_WithDefaultCurrency_ShouldUseVND()
    {
        // Arrange
        var amount = 100m;

        // Act
        var money = new Money(amount);

        // Assert
        money.Amount.Should().Be(amount);
        money.Currency.Should().Be("VND");
    }

    [Fact]
    public void Constructor_WithNegativeAmount_ShouldThrowDomainException()
    {
        // Arrange
        var amount = -100m;

        // Act
        Action act = () => new Money(amount);

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("Amount cannot be negative");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_WithInvalidCurrency_ShouldThrowDomainException(string invalidCurrency)
    {
        // Arrange
        var amount = 100m;

        // Act
        Action act = () => new Money(amount, invalidCurrency);

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("Currency is required");
    }

    [Theory]
    [InlineData("US")]
    [InlineData("USDD")]
    public void Constructor_WithInvalidCurrencyLength_ShouldThrowDomainException(string invalidCurrency)
    {
        // Arrange
        var amount = 100m;

        // Act
        Action act = () => new Money(amount, invalidCurrency);

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("Currency must be a 3-letter code");
    }

    [Fact]
    public void Constructor_ShouldNormalizeCurrencyToUpperCase()
    {
        // Arrange
        var amount = 100m;
        var currency = "usd";

        // Act
        var money = new Money(amount, currency);

        // Assert
        money.Currency.Should().Be("USD");
    }

    [Fact]
    public void Equals_WithSameValues_ShouldReturnTrue()
    {
        // Arrange
        var money1 = new Money(100m, "VND");
        var money2 = new Money(100m, "VND");

        // Act & Assert
        money1.Should().Be(money2);
        (money1 == money2).Should().BeTrue();
    }

    [Fact]
    public void Equals_WithDifferentAmounts_ShouldReturnFalse()
    {
        // Arrange
        var money1 = new Money(100m, "VND");
        var money2 = new Money(200m, "VND");

        // Act & Assert
        money1.Should().NotBe(money2);
        (money1 != money2).Should().BeTrue();
    }

    [Fact]
    public void Equals_WithDifferentCurrencies_ShouldReturnFalse()
    {
        // Arrange
        var money1 = new Money(100m, "VND");
        var money2 = new Money(100m, "USD");

        // Act & Assert
        money1.Should().NotBe(money2);
        (money1 != money2).Should().BeTrue();
    }

    [Fact]
    public void Add_WithSameCurrency_ShouldReturnSum()
    {
        // Arrange
        var money1 = new Money(100m, "VND");
        var money2 = new Money(50m, "VND");

        // Act
        var result = money1 + money2;

        // Assert
        result.Amount.Should().Be(150m);
        result.Currency.Should().Be("VND");
    }

    [Fact]
    public void Add_WithDifferentCurrencies_ShouldThrowDomainException()
    {
        // Arrange
        var money1 = new Money(100m, "VND");
        var money2 = new Money(50m, "USD");

        // Act
        Action act = () => { var result = money1 + money2; };

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("*different currencies*");
    }

    [Fact]
    public void Subtract_WithSameCurrency_ShouldReturnDifference()
    {
        // Arrange
        var money1 = new Money(100m, "VND");
        var money2 = new Money(30m, "VND");

        // Act
        var result = money1 - money2;

        // Assert
        result.Amount.Should().Be(70m);
        result.Currency.Should().Be("VND");
    }

    [Fact]
    public void Subtract_WithDifferentCurrencies_ShouldThrowDomainException()
    {
        // Arrange
        var money1 = new Money(100m, "VND");
        var money2 = new Money(30m, "USD");

        // Act
        Action act = () => { var result = money1 - money2; };

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("*different currencies*");
    }

    [Fact]
    public void Subtract_ResultingInNegative_ShouldThrowDomainException()
    {
        // Arrange
        var money1 = new Money(50m, "VND");
        var money2 = new Money(100m, "VND");

        // Act
        Action act = () => { var result = money1 - money2; };

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("Amount cannot be negative");
    }

    [Fact]
    public void Multiply_WithPositiveMultiplier_ShouldReturnProduct()
    {
        // Arrange
        var money = new Money(100m, "VND");
        var multiplier = 3m;

        // Act
        var result = money * multiplier;

        // Assert
        result.Amount.Should().Be(300m);
        result.Currency.Should().Be("VND");
    }

    [Fact]
    public void Multiply_WithMultiplierOnLeft_ShouldReturnProduct()
    {
        // Arrange
        var money = new Money(100m, "VND");
        var multiplier = 3m;

        // Act
        var result = multiplier * money;

        // Assert
        result.Amount.Should().Be(300m);
        result.Currency.Should().Be("VND");
    }

    [Fact]
    public void Divide_WithPositiveDivisor_ShouldReturnQuotient()
    {
        // Arrange
        var money = new Money(100m, "VND");
        var divisor = 4m;

        // Act
        var result = money / divisor;

        // Assert
        result.Amount.Should().Be(25m);
        result.Currency.Should().Be("VND");
    }

    [Fact]
    public void Divide_ByZero_ShouldThrowDomainException()
    {
        // Arrange
        var money = new Money(100m, "VND");
        var divisor = 0m;

        // Act
        Action act = () => { var result = money / divisor; };

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("Cannot divide money by zero");
    }

    [Fact]
    public void GreaterThan_WithSameCurrency_ShouldCompareCorrectly()
    {
        // Arrange
        var money1 = new Money(100m, "VND");
        var money2 = new Money(50m, "VND");

        // Act & Assert
        (money1 > money2).Should().BeTrue();
        (money2 > money1).Should().BeFalse();
    }

    [Fact]
    public void GreaterThan_WithDifferentCurrencies_ShouldThrowDomainException()
    {
        // Arrange
        var money1 = new Money(100m, "VND");
        var money2 = new Money(50m, "USD");

        // Act
        Action act = () => { var result = money1 > money2; };

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("*different currencies*");
    }

    [Fact]
    public void LessThan_WithSameCurrency_ShouldCompareCorrectly()
    {
        // Arrange
        var money1 = new Money(50m, "VND");
        var money2 = new Money(100m, "VND");

        // Act & Assert
        (money1 < money2).Should().BeTrue();
        (money2 < money1).Should().BeFalse();
    }

    [Fact]
    public void GreaterThanOrEqual_WithSameCurrency_ShouldCompareCorrectly()
    {
        // Arrange
        var money1 = new Money(100m, "VND");
        var money2 = new Money(100m, "VND");
        var money3 = new Money(50m, "VND");

        // Act & Assert
        (money1 >= money2).Should().BeTrue();
        (money1 >= money3).Should().BeTrue();
        (money3 >= money1).Should().BeFalse();
    }

    [Fact]
    public void LessThanOrEqual_WithSameCurrency_ShouldCompareCorrectly()
    {
        // Arrange
        var money1 = new Money(50m, "VND");
        var money2 = new Money(50m, "VND");
        var money3 = new Money(100m, "VND");

        // Act & Assert
        (money1 <= money2).Should().BeTrue();
        (money1 <= money3).Should().BeTrue();
        (money3 <= money1).Should().BeFalse();
    }

    [Fact]
    public void ToString_ShouldReturnFormattedString()
    {
        // Arrange
        var money = new Money(1234.56m, "VND");

        // Act
        var result = money.ToString();

        // Assert
        result.Should().Be("1,234.56 VND");
    }

    [Fact]
    public void Zero_ShouldCreateZeroMoney()
    {
        // Act
        var money = Money.Zero();

        // Assert
        money.Amount.Should().Be(0m);
        money.Currency.Should().Be("VND");
    }

    [Fact]
    public void Zero_WithCurrency_ShouldCreateZeroMoneyWithSpecifiedCurrency()
    {
        // Act
        var money = Money.Zero("USD");

        // Assert
        money.Amount.Should().Be(0m);
        money.Currency.Should().Be("USD");
    }

    [Fact]
    public void GetHashCode_WithEqualValues_ShouldReturnSameHashCode()
    {
        // Arrange
        var money1 = new Money(100m, "VND");
        var money2 = new Money(100m, "VND");

        // Act & Assert
        money1.GetHashCode().Should().Be(money2.GetHashCode());
    }
}
