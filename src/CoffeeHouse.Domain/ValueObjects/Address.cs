using CoffeeHouse.Domain.Exceptions;

namespace CoffeeHouse.Domain.ValueObjects;

/// <summary>
/// Value object representing a physical address
/// </summary>
public class Address : ValueObject
{
    /// <summary>
    /// Street address (house number, street name)
    /// </summary>
    public string Street { get; }

    /// <summary>
    /// District or ward
    /// </summary>
    public string District { get; }

    /// <summary>
    /// City or province
    /// </summary>
    public string City { get; }

    /// <summary>
    /// Private constructor for EF Core
    /// </summary>
    private Address()
    {
        Street = string.Empty;
        District = string.Empty;
        City = string.Empty;
    }

    /// <summary>
    /// Creates a new Address value object
    /// </summary>
    /// <param name="street">Street address</param>
    /// <param name="district">District or ward</param>
    /// <param name="city">City or province</param>
    /// <exception cref="DomainException">Thrown when any component is invalid</exception>
    public Address(string street, string district, string city)
    {
        ValidateStreet(street);
        ValidateDistrict(district);
        ValidateCity(city);

        Street = street.Trim();
        District = district.Trim();
        City = city.Trim();
    }

    /// <summary>
    /// Gets the equality components for value object comparison
    /// </summary>
    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Street.ToLowerInvariant();
        yield return District.ToLowerInvariant();
        yield return City.ToLowerInvariant();
    }

    /// <summary>
    /// Returns the full address as a formatted string
    /// </summary>
    public override string ToString()
    {
        return $"{Street}, {District}, {City}";
    }

    /// <summary>
    /// Returns the full address with each component on a new line
    /// </summary>
    public string ToMultiLineString()
    {
        return $"{Street}\n{District}\n{City}";
    }

    /// <summary>
    /// Validates the street component
    /// </summary>
    private static void ValidateStreet(string street)
    {
        if (string.IsNullOrWhiteSpace(street))
        {
            throw new DomainException("Street address is required");
        }

        if (street.Length > 255)
        {
            throw new DomainException("Street address must not exceed 255 characters");
        }
    }

    /// <summary>
    /// Validates the district component
    /// </summary>
    private static void ValidateDistrict(string district)
    {
        if (string.IsNullOrWhiteSpace(district))
        {
            throw new DomainException("District is required");
        }

        if (district.Length > 100)
        {
            throw new DomainException("District must not exceed 100 characters");
        }
    }

    /// <summary>
    /// Validates the city component
    /// </summary>
    private static void ValidateCity(string city)
    {
        if (string.IsNullOrWhiteSpace(city))
        {
            throw new DomainException("City is required");
        }

        if (city.Length > 100)
        {
            throw new DomainException("City must not exceed 100 characters");
        }
    }

    /// <summary>
    /// Creates an Address from a single string (attempts to parse)
    /// </summary>
    /// <param name="fullAddress">Full address string (comma-separated)</param>
    /// <returns>Address value object</returns>
    /// <exception cref="DomainException">Thrown when address cannot be parsed</exception>
    public static Address FromString(string fullAddress)
    {
        if (string.IsNullOrWhiteSpace(fullAddress))
        {
            throw new DomainException("Address string is required");
        }

        var parts = fullAddress.Split(',', StringSplitOptions.TrimEntries);

        if (parts.Length < 3)
        {
            throw new DomainException("Address must contain at least street, district, and city separated by commas");
        }

        // Take the first part as street, second as district, and join the rest as city
        var street = parts[0];
        var district = parts[1];
        var city = string.Join(", ", parts.Skip(2));

        return new Address(street, district, city);
    }
}
