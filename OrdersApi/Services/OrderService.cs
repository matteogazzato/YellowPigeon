using OrdersApi.Contracts;
using OrdersApi.Data;
using OrdersApi.Exceptions;

namespace OrdersApi.Services;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _repository;

    public OrderService(IOrderRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    public async Task<CreateOrderResponse> CreateOrderAsync(
        CreateOrderRequest request,
        CancellationToken cancellationToken = default)
    {
        ValidateRequest(request);

        var itemsList = request.Items.ToList();
        var totalAmount = itemsList.Sum(item => item.Quantity * item.UnitPrice);
        var createdAtUtc = DateTime.UtcNow;

        var orderToPersist = new OrderToPersist(
            CustomerId: request.CustomerId,
            Currency: request.Currency,
            TotalAmount: totalAmount,
            CreatedAtUtc: createdAtUtc,
            Items: itemsList.Select(item => new OrderItemToPersist(
                ProductCode: item.ProductCode,
                Quantity: item.Quantity,
                UnitPrice: item.UnitPrice
            )).ToList());

        var result = await _repository.CreateOrderAsync(orderToPersist, cancellationToken);

        return new CreateOrderResponse(
            OrderId: result.OrderId,
            CreatedAtUtc: result.CreatedAtUtc,
            CustomerId: request.CustomerId,
            Currency: request.Currency,
            TotalAmount: totalAmount,
            ItemsCount: itemsList.Count);
    }

    private static void ValidateRequest(CreateOrderRequest request)
    {
        var errors = new Dictionary<string, string[]>();

        if (request.CustomerId <= 0)
            errors["customerId"] = new[] { "CustomerId must be greater than zero." };

        if (string.IsNullOrWhiteSpace(request.Currency))
            errors["currency"] = new[] { "Currency is required and cannot be empty." };

        if (request.Items == null)
        {
            errors["items"] = new[] { "Items collection is required." };
        }
        else
        {
            var itemsList = request.Items.ToList();
            if (itemsList.Count == 0)
            {
                errors["items"] = new[] { "At least one item is required." };
            }
            else
            {
                foreach (var kvp in ValidateItems(itemsList))
                    errors[kvp.Key] = kvp.Value;
            }
        }

        if (errors.Count > 0)
            throw new OrderValidationException("Request validation failed. See errors for details.", errors);
    }

    private static Dictionary<string, string[]> ValidateItems(List<CreateOrderItemRequest> items)
    {
        var errors = new Dictionary<string, string[]>();
        for (int i = 0; i < items.Count; i++)
        {
            var item = items[i];
            var itemErrors = new List<string>();
            if (string.IsNullOrWhiteSpace(item.ProductCode))
                itemErrors.Add("ProductCode is required and cannot be empty.");
            if (item.Quantity <= 0)
                itemErrors.Add("Quantity must be greater than zero.");
            if (item.UnitPrice <= 0)
                itemErrors.Add("UnitPrice must be greater than zero.");
            if (itemErrors.Count > 0)
                errors[$"items[{i}]"] = itemErrors.ToArray();
        }
        return errors;
    }
}
