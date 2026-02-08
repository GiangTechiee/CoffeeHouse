using System.Text.RegularExpressions;

namespace CoffeeHouse.Application.Common.Helpers;

public static class InputSanitizer
{
    private static readonly Regex HtmlTagRegex = new Regex("<.*?>", RegexOptions.Compiled);
    private static readonly Regex ScriptTagRegex = new Regex("<script.*?>.*?</script>", RegexOptions.Compiled | RegexOptions.IgnoreCase);
    private static readonly Regex SqlInjectionRegex = new Regex(
        @"(\b(SELECT|INSERT|UPDATE|DELETE|DROP|ALTER|CREATE|EXEC|UNION|WHERE|OR|AND)\b)|(''')|(;)|(\-\-)|(/\*.*\*/)",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    public static string SanitizeHtml(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            return input;
        }

        return HtmlTagRegex.Replace(input, string.Empty);
    }

    public static string SanitizeScript(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            return input;
        }

        return ScriptTagRegex.Replace(input, string.Empty);
    }

    public static string SanitizeSql(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            return input;
        }

        return SqlInjectionRegex.Replace(input, string.Empty);
    }

    public static string SanitizeAll(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            return input;
        }

        input = SanitizeHtml(input);
        input = SanitizeScript(input);
        input = SanitizeSql(input);

        return input.Trim();
    }

    public static string SanitizeEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            return email;
        }

        email = email.ToLower().Trim();
        return email;
    }

    public static string SanitizePhoneNumber(string phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(phoneNumber))
        {
            return phoneNumber;
        }

        // Remove all non-digit characters
        return Regex.Replace(phoneNumber, @"[^\d]", string.Empty);
    }
}
