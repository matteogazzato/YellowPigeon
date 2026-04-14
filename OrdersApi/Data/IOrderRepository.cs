namespace OrdersApi.Data;

public interface IOrderRepository
{
    Task<CreateOrderDbResult> CreateOrderAsync(
        OrderToPersist order,
        CancellationToken cancellationToken = default);
}

public record OrderToPersist(
    int CustomerId,
    string Currency,
    decimal TotalAmount,
    DateTime CreatedAtUtc,
    List<OrderItemToPersist> Items);

public record OrderItemToPersist(
    string ProductCode,
    int Quantity,
    decimal UnitPrice);

public record CreateOrderDbResult(int OrderId, DateTime CreatedAtUtc);
