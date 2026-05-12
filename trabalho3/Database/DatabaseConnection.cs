using System.Data.Common;
using System.Runtime.InteropServices;
using Npgsql;


public class DatabaseConnection
{
    private readonly string _connectionString;

    public DatabaseConnection(string connectionString)
    {
        _connectionString = connectionString;
    }

    public DbConnection GetConnection()
    {
        var conn = new NpgsqlConnection(_connectionString);
        conn.Open();
        Console.WriteLine("Conexão com o banco de dados estabelecida com sucesso.");
        return conn;
    }
}