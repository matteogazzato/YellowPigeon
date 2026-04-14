using OrdersApi.Contracts;

namespace OrdersApi.Services;

public interface IOrderService
{
    Task<CreateOrderResponse> CreateOrderAsync(
        CreateOrderRequest request,
        CancellationToken cancellationToken = default);
}
