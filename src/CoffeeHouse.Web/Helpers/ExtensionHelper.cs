
using System.Text.Json;

namespace CoffeeHouse.Helpers
{
    public static class ExtensionHelper
    {
        public static void Set<T>(this ISession session, string key, T value)
        {
            session.SetString(key, JsonSerializer.Serialize(value));
        }

        public static T Get<T>(this ISession session, string key)
        {
            var value = session.GetString(key);

            if (session == null)
            {
                // X? lý tình hu?ng phiên không t?n t?i
                return default;
            }

            if (string.IsNullOrEmpty(value))
            {
                // X? lý tình hu?ng chu?i JSON tr?ng
                return default;
            }

            try
            {
                return JsonSerializer.Deserialize<T>(value);
            }
            catch (JsonException ex)
            {
                Console.WriteLine(ex.Message);
                return default;
            }
        }
    }
}
