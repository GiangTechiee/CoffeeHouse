using Npgsql;

if (args.Length < 1)
{
    Console.WriteLine("Missing connection string.");
    Environment.Exit(1);
}

var connStr = args[0];

try
{
    using var conn = new NpgsqlConnection(connStr);
    conn.Open();
    using var cmd = new NpgsqlCommand("SELECT \"StoreId\" FROM \"CafeStores\" ORDER BY \"StoreId\" LIMIT 1", conn);
    var result = cmd.ExecuteScalar();
    if (result == null)
    {
        Console.WriteLine("0");
    }
    else
    {
        Console.WriteLine(result);
    }
}
catch (Exception ex)
{
    Console.WriteLine($"ERROR: {ex.Message}");
    Environment.Exit(2);
}
