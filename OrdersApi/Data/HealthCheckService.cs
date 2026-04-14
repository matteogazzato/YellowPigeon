namespace OrdersApi.Data;

using Microsoft.Data.SqlClient;

/// <summary>
/// Service for health checks and diagnostics.
/// 
/// Provides database connectivity testing and system health information.
/// This is useful for monitoring during development and deployment.
/// </summary>
public class HealthCheckService
{
    private readonly string _connectionString;

    public HealthCheckService(IConfiguration configuration)
    {
        if (configuration == null)
            throw new ArgumentNullException(nameof(configuration));

        var connectionString = configuration.GetConnectionString("OrdersDb");
        if (string.IsNullOrWhiteSpace(connectionString))
            throw new InvalidOperationException(
                "Connection string 'OrdersDb' not found in configuration.");

        _connectionString = connectionString;
    }

    /// <summary>
    /// Checks the health of the application and database connectivity.
    /// 
    /// Returns:
    /// - HealthStatus.Healthy(200) if the database is reachable
    /// - HealthStatus.Unhealthy(503) if the database is unreachable or any error occurs
    /// </summary>
    public async Task<HealthStatus> CheckHealthAsync()
    {
        try
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            return HealthStatus.Healthy(
                database: "OrdersLab",
                server: "localhost,1433",
                message: "Successfully connected to the database");
        }
        catch (Exception ex)
        {
            return HealthStatus.Unhealthy(error: ex.Message);
        }
    }
}

/// <summary>
/// Represents the result of a health check.
/// </summary>
public class HealthStatus
{
    public string Status { get; }
    public int StatusCode { get; }
    public Dictionary<string, object?> Details { get; }

    private HealthStatus(string status, int statusCode, Dictionary<string, object?> details)
    {
        Status = status;
        StatusCode = statusCode;
        Details = details;
    }

    public static HealthStatus Healthy(string database, string server, string message)
    {
        return new HealthStatus(
            status: "Healthy",
            statusCode: 200,
            details: new()
            {
                { "database", database },
                { "server", server },
                { "message", message }
            });
    }

    public static HealthStatus Unhealthy(string error)
    {
        return new HealthStatus(
            status: "Unhealthy",
            statusCode: 503,
            details: new()
            {
                { "error", error }
            });
    }
}
