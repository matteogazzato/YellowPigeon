namespace OrdersApi.Services;

/// <summary>
/// Repository interface for order data access.
/// </summary>
public interface IOrderRepository
{
    /// <summary>
    /// Retrieves an order by its ID.
    /// </summary>
    /// <param name="id">The order ID.</param>
    /// <returns>The order if found, null otherwise.</returns>
    Task<Contracts.Order?> GetById(int id);
}
