using Microsoft.Data.SqlClient;
using OrdersApi.Exceptions;

namespace OrdersApi.Data;

public class OrderRepository : IOrderRepository
{
    private readonly string _connectionString;
    private readonly ILogger<OrderRepository> _logger;

    public OrderRepository(IConfiguration configuration, ILogger<OrderRepository> logger)
    {
        _connectionString = configuration.GetConnectionString("OrdersDb")
            ?? throw new InvalidOperationException("Connection string 'OrdersDb' is not configured.");
        _logger = logger;
    }

    public async Task<CreateOrderDbResult> CreateOrderAsync(
        OrderToPersist order,
        CancellationToken cancellationToken = default)
    {
        using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);
        using var transaction = connection.BeginTransaction();

        try
        {
            // Insert order header; CreatedAtUtc set by the service (no DB default)
            const string insertOrderSql = @"
                INSERT INTO Orders (CustomerId, Currency, TotalAmount, CreatedAtUtc)
                VALUES (@customerId, @currency, @totalAmount, @createdAtUtc);
                SELECT CAST(SCOPE_IDENTITY() AS INT);";

            int orderId;
            using (var cmd = new SqlCommand(insertOrderSql, connection, transaction))
            {
                cmd.Parameters.AddWithValue("@customerId", order.CustomerId);
                cmd.Parameters.AddWithValue("@currency", order.Currency);
                cmd.Parameters.AddWithValue("@totalAmount", order.TotalAmount);
                cmd.Parameters.AddWithValue("@createdAtUtc", order.CreatedAtUtc);

                var result = await cmd.ExecuteScalarAsync(cancellationToken);
                orderId = (int)(result ?? throw new InvalidOperationException("Failed to retrieve generated OrderId."));
            }

            // Insert each item
            const string insertItemSql = @"
                INSERT INTO OrderItems (OrderId, ProductCode, Quantity, UnitPrice)
                VALUES (@orderId, @productCode, @quantity, @unitPrice);";

            foreach (var item in order.Items)
            {
                using var cmd = new SqlCommand(insertItemSql, connection, transaction);
                cmd.Parameters.AddWithValue("@orderId", orderId);
                cmd.Parameters.AddWithValue("@productCode", item.ProductCode);
                cmd.Parameters.AddWithValue("@quantity", item.Quantity);
                cmd.Parameters.AddWithValue("@unitPrice", item.UnitPrice);
                await cmd.ExecuteNonQueryAsync(cancellationToken);
            }

            await transaction.CommitAsync(cancellationToken);
            _logger.LogInformation("Created order {OrderId} with {ItemCount} item(s) for customer {CustomerId}",
                orderId, order.Items.Count, order.CustomerId);
            return new CreateOrderDbResult(orderId, order.CreatedAtUtc);
        }
        catch (SqlException sqlEx)
        {
            await transaction.RollbackAsync(CancellationToken.None);
            bool isConflict = sqlEx.Number == 547;
            string detail = $"SqlException {sqlEx.Number}: {sqlEx.Message}";
            _logger.LogWarning(sqlEx, "Persistence failed. IsConflict={IsConflict}. Detail: {Detail}", isConflict, detail);
            throw new OrderPersistenceException("A database constraint prevented the order from being saved.", isConflict, detail, sqlEx);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(CancellationToken.None);
            _logger.LogError(ex, "Unexpected error during order persistence");
            throw new OrderPersistenceException("An unexpected database error occurred.", isConflict: false, detail: ex.Message, innerException: ex);
        }
    }
}
