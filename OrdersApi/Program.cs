// ─────────────────────────────────────────────────────────────────────────────
// Program.cs — Minimal application startup.
// During the demo this file will be extended to add services and middleware.
// ─────────────────────────────────────────────────────────────────────────────

using OrdersApi.Data;
using OrdersApi.Services;

var builder = WebApplication.CreateBuilder(args);

// Add basic HTTP services
builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddProblemDetails();

// Register health check service for diagnostics
builder.Services.AddSingleton<HealthCheckService>();

// Register order creation services
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IOrderService, OrderService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

// ─────────────────────────────────────────────────────────────────────────────
// Diagnostic endpoint for testing database connectivity
// ─────────────────────────────────────────────────────────────────────────────
app.MapGet("/health", async (HealthCheckService healthCheck) =>
{
    var status = await healthCheck.CheckHealthAsync();
    return Results.Json(
        new { status = status.Status, details = status.Details },
        statusCode: status.StatusCode);
})
.WithName("Health")
.WithOpenApi()
.Produces(200)
.Produces(503);

app.Run();


