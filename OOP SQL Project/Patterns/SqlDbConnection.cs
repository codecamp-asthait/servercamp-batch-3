using Microsoft.Data.SqlClient;

namespace CoffeeShopSystem.Patterns;

public sealed class SqlDbConnection
{
    private static SqlDbConnection? _instance;
    private static readonly object _lock = new object();
    private readonly string _connectionString;

    private SqlDbConnection()
    {
        _connectionString = "Server=localhost;Database=CoffeeShopDB;Trusted_Connection=True;TrustServerCertificate=True;";
    }

    public static SqlDbConnection instance
    {
        get
        {
            lock (_lock)
            {
                if (_instance == null)
                {
                    _instance = new SqlDbConnection();
                }
                return _instance;
            }
        }
    }

    public SqlConnection CreateConnection()
    {
        return new SqlConnection(_connectionString);
    }
}