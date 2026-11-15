using ClinicApi.Data;
using Dapper;
using Microsoft.AspNetCore.Mvc;

namespace ClinicApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HealthController : ControllerBase
{
    private readonly IDbConnectionFactory _connectionFactory;
    private readonly ILogger<HealthController> _logger;

    public HealthController(IDbConnectionFactory connectionFactory, ILogger<HealthController> logger)
    {
        _connectionFactory = connectionFactory;
        _logger = logger;
    }

    /// <summary>
    /// Basic API health check endpoint.
    /// </summary>
    [HttpGet]
    public IActionResult GetHealth()
    {
        return Ok(new
        {
            status = "Healthy",
            timestamp = DateTime.UtcNow,
            service = "Clinic Appointment API",
            version = "1.0.0"
        });
    }

    /// <summary>
    /// Database connectivity health check using Dapper and Npgsql.
    /// </summary>
    [HttpGet("database")]
    public async Task<IActionResult> GetDatabaseHealth()
    {
        if (!_connectionFactory.IsPasswordConfigured)
        {
            _logger.LogWarning("Health check: DB_PASSWORD environment variable is not configured.");
            return StatusCode(StatusCodes.Status503ServiceUnavailable, new
            {
                status = "Degraded",
                database = _connectionFactory.DatabaseName,
                connected = false,
                message = "DB_PASSWORD environment variable is not configured. Please set the DB_PASSWORD environment variable.",
                timestamp = DateTime.UtcNow
            });
        }

        try
        {
            using var connection = _connectionFactory.CreateConnection();
            connection.Open();

            // Check connectivity and query current database and schema via Dapper
            var queryResult = await connection.QueryFirstOrDefaultAsync<dynamic>(
                "SELECT current_database() AS current_db, current_schema() AS current_schema, 1 AS ping;");

            return Ok(new
            {
                status = "Healthy",
                database = (string?)queryResult?.current_db,
                schema = (string?)queryResult?.current_schema,
                ping = (int?)queryResult?.ping == 1 ? "Success" : "Failed",
                connected = true,
                timestamp = DateTime.UtcNow
            });
        }
        catch (Exception ex)
        {
            _logger.LogError("Database health check failed: {Message}", ex.Message);
            return StatusCode(StatusCodes.Status503ServiceUnavailable, new
            {
                status = "Unhealthy",
                database = _connectionFactory.DatabaseName,
                connected = false,
                message = "Failed to connect to PostgreSQL database.",
                error = ex.Message,
                timestamp = DateTime.UtcNow
            });
        }
    }
}

