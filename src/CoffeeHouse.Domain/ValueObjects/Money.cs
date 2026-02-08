using CoffeeHouse.Domain.Exceptions;

namespace CoffeeHouse.Domain.ValueObjects;

/// <summary>
/// Value object representing a monetary amount with currency
/// </summary>
public class Money : ValueObject
{
    /// <summary>
    /// The monetary amount
    /// </summary>
    public decimal Amount { get; }

    /// <summary>
    /// The currency code (e.g., "VND", "USD")
    /// </summary>
    public string Currency { get; }

    /// <summary>
    /// Private constructor for EF Core
    /// </summary>
    private Money()
    {
        Currency = "VND";
    }

    /// <summary>
    /// Creates a new Money value object
    /// </summary>
    /// <param name="amount">The monetary amount</param>
    /// <param name="currency">The currency code (defaults to VND)</param>
    /// <exception cref="DomainException">Thrown when amount is negative or currency is invalid</exception>
    public Money(decimal amount, string currency = "VND")
    {
        if (amount < 0)
        {
            throw new DomainException("Amount cannot be negative");
        }

        if (string.IsNullOrWhiteSpace(currency))
        {
            throw new DomainException("Currency is required");
        }

        if (currency.Length != 3)
        {
            throw new DomainException("Currency must be a 3-letter code");
        }

        Amount = amount;
        Currency = currency.ToUpperInvariant();
    }

    /// <summary>
    /// Gets the equality components for value object comparison
    /// </summary>
    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Amount;
        yield return Currency;
    }

    /// <summary>
    /// Adds two Money values (must have same currency)
    /// </summary>
    /// <param name="left">First money value</param>
    /// <param name="right">Second money value</param>
    /// <returns>Sum of the two money values</returns>
    /// <exception cref="DomainException">Thrown when currencies don't match</exception>
    public static Money operator +(Money left, Money right)
    {
        if (left.Currency != right.Currency)
        {
            throw new DomainException($"Cannot add money with different currencies: {left.Currency} and {right.Currency}");
        }

        return new Money(left.Amount + right.Amount, left.Currency);
    }

    /// <summary>
    /// Subtracts two Money values (must have same currency)
    /// </summary>
    /// <param name="left">First money value</param>
    /// <param name="right">Second money value</param>
    /// <returns>Difference of the two money values</returns>
    /// <exception cref="DomainException">Thrown when currencies don't match or result is negative</exception>
    public static Money operator -(Money left, Money right)
    {
        if (left.Currency != right.Currency)
        {
            throw new DomainException($"Cannot subtract money with different currencies: {left.Currency} and {right.Currency}");
        }

        return new Money(left.Amount - right.Amount, left.Currency);
    }

    /// <summary>
    /// Multiplies a Money value by a scalar
    /// </summary>
    /// <param name="money">Money value</param>
    /// <param name="multiplier">Multiplier</param>
    /// <returns>Product of money and multiplier</returns>
    public static Money operator *(Money money, decimal multiplier)
    {
        return new Money(money.Amount * multiplier, money.Currency);
    }

    /// <summary>
    /// Multiplies a Money value by a scalar
    /// </summary>
    /// <param name="multiplier">Multiplier</param>
    /// <param name="money">Money value</param>
    /// <returns>Product of money and multiplier</returns>
    public static Money operator *(decimal multiplier, Money money)
    {
        return money * multiplier;
    }

    /// <summary>
    /// Divides a Money value by a scalar
    /// </summary>
    /// <param name="money">Money value</param>
    /// <param name="divisor">Divisor</param>
    /// <returns>Quotient of money and divisor</returns>
    /// <exception cref="DomainException">Thrown when divisor is zero</exception>
    public static Money operator /(Money money, decimal divisor)
    {
        if (divisor == 0)
        {
            throw new DomainException("Cannot divide money by zero");
        }

        return new Money(money.Amount / divisor, money.Currency);
    }

    /// <summary>
    /// Compares if left money is greater than right money
    /// </summary>
    /// <param name="left">First money value</param>
    /// <param name="right">Second money value</param>
    /// <returns>True if left is greater than right</returns>
    /// <exception cref="DomainException">Thrown when currencies don't match</exception>
    public static bool operator >(Money left, Money right)
    {
        if (left.Currency != right.Currency)
        {
            throw new DomainException($"Cannot compare money with different currencies: {left.Currency} and {right.Currency}");
        }

        return left.Amount > right.Amount;
    }

    /// <summary>
    /// Compares if left money is less than right money
    /// </summary>
    /// <param name="left">First money value</param>
    /// <param name="right">Second money value</param>
    /// <returns>True if left is less than right</returns>
    /// <exception cref="DomainException">Thrown when currencies don't match</exception>
    public static bool operator <(Money left, Money right)
    {
        if (left.Currency != right.Currency)
        {
            throw new DomainException($"Cannot compare money with different currencies: {left.Currency} and {right.Currency}");
        }

        return left.Amount < right.Amount;
    }

    /// <summary>
    /// Compares if left money is greater than or equal to right money
    /// </summary>
    /// <param name="left">First money value</param>
    /// <param name="right">Second money value</param>
    /// <returns>True if left is greater than or equal to right</returns>
    /// <exception cref="DomainException">Thrown when currencies don't match</exception>
    public static bool operator >=(Money left, Money right)
    {
        if (left.Currency != right.Currency)
        {
            throw new DomainException($"Cannot compare money with different currencies: {left.Currency} and {right.Currency}");
        }

        return left.Amount >= right.Amount;
    }

    /// <summary>
    /// Compares if left money is less than or equal to right money
    /// </summary>
    /// <param name="left">First money value</param>
    /// <param name="right">Second money value</param>
    /// <returns>True if left is less than or equal to right</returns>
    /// <exception cref="DomainException">Thrown when currencies don't match</exception>
    public static bool operator <=(Money left, Money right)
    {
        if (left.Currency != right.Currency)
        {
            throw new DomainException($"Cannot compare money with different currencies: {left.Currency} and {right.Currency}");
        }

        return left.Amount <= right.Amount;
    }

    /// <summary>
    /// Returns a string representation of the money value
    /// </summary>
    public override string ToString()
    {
        return $"{Amount:N2} {Currency}";
    }

    /// <summary>
    /// Creates a zero money value in the specified currency
    /// </summary>
    /// <param name="currency">Currency code</param>
    /// <returns>Zero money value</returns>
    public static Money Zero(string currency = "VND")
    {
        return new Money(0, currency);
    }
}
