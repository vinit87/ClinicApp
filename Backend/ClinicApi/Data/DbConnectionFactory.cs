using System.Data;
using Npgsql;

namespace ClinicApi.Data;

public class DbConnectionFactory : IDbConnectionFactory
{
    private readonly IConfiguration _configuration;
    private readonly string _host;
    private readonly int _port;
    private readonly string _database;
    private readonly string _username;
    private readonly string _schema;

    public DbConnectionFactory(IConfiguration configuration)
    {
        _configuration = configuration;
        _host = _configuration["DatabaseConfig:Host"] ?? "localhost";
        _port = int.TryParse(_configuration["DatabaseConfig:Port"], out var port) ? port : 5432;
        _database = _configuration["DatabaseConfig:Database"] ?? "clinic_appointment_db";
        _username = _configuration["DatabaseConfig:Username"] ?? "postgres";
        _schema = _configuration["DatabaseConfig:Schema"] ?? "clinic";
    }

    public string DatabaseName => _database;
    public string SchemaName => _schema;

    public bool IsPasswordConfigured =>
        !string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("DB_PASSWORD"));

    public IDbConnection CreateConnection()
    {
        var password = Environment.GetEnvironmentVariable("DB_PASSWORD");
        if (string.IsNullOrWhiteSpace(password))
        {
            throw new InvalidOperationException("DB_PASSWORD environment variable is not configured. Please set the DB_PASSWORD environment variable.");
        }

        var builder = new NpgsqlConnectionStringBuilder
        {
            Host = _host,
            Port = _port,
            Database = _database,
            Username = _username,
            Password = password,
            SearchPath = _schema,
            Pooling = true,
            Timeout = 5
        };

        return new NpgsqlConnection(builder.ConnectionString);
    }

    public IDbConnection CreateServerConnection()
    {
        var password = Environment.GetEnvironmentVariable("DB_PASSWORD");
        if (string.IsNullOrWhiteSpace(password))
        {
            throw new InvalidOperationException("DB_PASSWORD environment variable is not configured. Please set the DB_PASSWORD environment variable.");
        }

        var builder = new NpgsqlConnectionStringBuilder
        {
            Host = _host,
            Port = _port,
            Database = "postgres", // Default database to check/create clinic_appointment_db
            Username = _username,
            Password = password,
            Pooling = false,
            Timeout = 5
        };

        return new NpgsqlConnection(builder.ConnectionString);
    }
}

