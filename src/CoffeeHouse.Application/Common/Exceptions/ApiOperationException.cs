namespace CoffeeHouse.Application.Common.Exceptions;

/// <summary>
/// Represents an API-level operation failure with an error code and optional details.
/// </summary>
public class ApiOperationException : Exception
{
    public string Code { get; }
    public object? Details { get; }

    public ApiOperationException(string code, string message, object? details = null)
        : base(message)
    {
        Code = code;
        Details = details;
    }
}
