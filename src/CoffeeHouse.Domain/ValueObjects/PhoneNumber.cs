using System.Text.RegularExpressions;
using CoffeeHouse.Domain.Exceptions;

namespace CoffeeHouse.Domain.ValueObjects;

/// <summary>
/// Value object representing a Vietnamese phone number
/// </summary>
public class PhoneNumber : ValueObject
{
    /// <summary>
    /// The phone number value
    /// </summary>
    public string Value { get; }

    /// <summary>
    /// Regular expression for Vietnamese phone number validation
    /// Supports formats:
    /// - Mobile: 03x, 05x, 07x, 08x, 09x (10 digits)
    /// - Landline: 02x (10 digits)
    /// </summary>
    private static readonly Regex PhoneNumberRegex = new Regex(
        @"^(0[3|5|7|8|9]|02[0-9])[0-9]{8}$",
        RegexOptions.Compiled
    );

    /// <summary>
    /// Private constructor for EF Core
    /// </summary>
    private PhoneNumber()
    {
        Value = string.Empty;
    }

    /// <summary>
    /// Creates a new PhoneNumber value object
    /// </summary>
    /// <param name="value">Phone number string</param>
    /// <exception cref="DomainException">Thrown when phone number is invalid</exception>
    public PhoneNumber(string value)
    {
        ValidatePhoneNumber(value);
        Value = NormalizePhoneNumber(value);
    }

    /// <summary>
    /// Gets the equality components for value object comparison
    /// </summary>
    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    /// <summary>
    /// Returns the phone number as a string
    /// </summary>
    public override string ToString()
    {
        return Value;
    }

    /// <summary>
    /// Returns the phone number in a formatted display format
    /// Example: 0912345678 -> 091 234 5678
    /// </summary>
    public string ToFormattedString()
    {
        if (Value.Length == 10)
        {
            return $"{Value.Substring(0, 3)} {Value.Substring(3, 3)} {Value.Substring(6, 4)}";
        }

        return Value;
    }

    /// <summary>
    /// Validates the phone number format
    /// </summary>
    private static void ValidatePhoneNumber(string phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(phoneNumber))
        {
            throw new DomainException("Phone number is required");
        }

        // Remove common formatting characters for validation
        var normalized = NormalizePhoneNumber(phoneNumber);

        if (!PhoneNumberRegex.IsMatch(normalized))
        {
            throw new DomainException(
                "Invalid Vietnamese phone number format. " +
                "Phone number must be 10 digits starting with 03, 05, 07, 08, 09 (mobile) or 02 (landline)"
            );
        }
    }

    /// <summary>
    /// Normalizes the phone number by removing formatting characters
    /// </summary>
    private static string NormalizePhoneNumber(string phoneNumber)
    {
        // Remove spaces, dashes, parentheses, and other common formatting characters
        return Regex.Replace(phoneNumber, @"[\s\-\(\)\.]", string.Empty);
    }

    /// <summary>
    /// Determines if the phone number is a mobile number
    /// </summary>
    public bool IsMobile()
    {
        if (Value.Length != 10)
            return false;

        var prefix = Value.Substring(0, 2);
        return prefix == "03" || prefix == "05" || prefix == "07" || prefix == "08" || prefix == "09";
    }

    /// <summary>
    /// Determines if the phone number is a landline number
    /// </summary>
    public bool IsLandline()
    {
        if (Value.Length != 10)
            return false;

        return Value.StartsWith("02");
    }

    /// <summary>
    /// Gets the network operator prefix (for mobile numbers)
    /// </summary>
    public string? GetOperatorPrefix()
    {
        if (!IsMobile())
            return null;

        return Value.Substring(0, 3);
    }

    /// <summary>
    /// Attempts to create a PhoneNumber from a string, returning null if invalid
    /// </summary>
    /// <param name="value">Phone number string</param>
    /// <returns>PhoneNumber if valid, null otherwise</returns>
    public static PhoneNumber? TryCreate(string value)
    {
        try
        {
            return new PhoneNumber(value);
        }
        catch (DomainException)
        {
            return null;
        }
    }
}
