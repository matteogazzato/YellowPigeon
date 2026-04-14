using System.ComponentModel.DataAnnotations;

namespace OrdersApi.Contracts;

/// <summary>
/// Request contract for creating a new order with items.
/// 
/// This DTO captures the client's intent to create an order and is validated
/// at the API boundary (model binding layer). Required fields must be present;
/// range/collection size validations are checked by the service layer before
/// attempting database persistence.
/// </summary>
public record CreateOrderRequest
{
    /// <summary>
    /// The customer ID associated with this order.
    /// Must be a valid customer that already exists in the Customers table (enforced by FK constraint).
    /// </summary>
    [Required(ErrorMessage = "customerId is required")]
    [Range(1, int.MaxValue, ErrorMessage = "customerId must be greater than 0")]
    public int CustomerId { get; init; }

    /// <summary>
    /// ISO 4217 currency code (e.g., "EUR", "USD", "GBP").
    /// Must be a non-empty string. Suggested length: 3 characters,
    /// though not strictly enforced to allow flexibility.
    /// </summary>
    [Required(ErrorMessage = "currency is required")]
    [StringLength(10, MinimumLength = 1, ErrorMessage = "currency must be a non-empty string")]
    public string Currency { get; init; } = string.Empty;

    /// <summary>
    /// Array of order items. Must contain at least one item to create a valid order.
    /// Each item specifies the product code, quantity, and unit price.
    /// </summary>
    [Required(ErrorMessage = "items array is required")]
    [MinLength(1, ErrorMessage = "items array must contain at least 1 item")]
    public CreateOrderItemRequest[] Items { get; init; } = Array.Empty<CreateOrderItemRequest>();
}

/// <summary>
/// Request contract for a single order item line.
/// 
/// Represents a product being purchased: the product identifier (productCode),
/// the quantity ordered, and the unit price at the time of order creation.
/// Quantity and UnitPrice have both API and database-level CHECK constraints.
/// </summary>
public record CreateOrderItemRequest
{
    /// <summary>
    /// Product code or SKU that identifies the item being ordered.
    /// Must be a non-empty string (typically alphanumeric with optional dashes/underscores).
    /// </summary>
    [Required(ErrorMessage = "productCode is required")]
    [StringLength(50, MinimumLength = 1, ErrorMessage = "productCode must be a non-empty string")]
    public string ProductCode { get; init; } = string.Empty;

    /// <summary>
    /// Quantity of the item being ordered.
    /// Must be a positive integer (> 0). Enforced at both API and database layers.
    /// </summary>
    [Required(ErrorMessage = "quantity is required")]
    [Range(1, int.MaxValue, ErrorMessage = "quantity must be greater than 0")]
    public int Quantity { get; init; }

    /// <summary>
    /// Unit price of the item at the time the order is created.
    /// Must be a positive decimal value (> 0). Represents the price per unit.
    /// The line total for this item = Quantity × UnitPrice.
    /// </summary>
    [Required(ErrorMessage = "unitPrice is required")]
    [Range(0.01, 999999.99, ErrorMessage = "unitPrice must be greater than 0")]
    public decimal UnitPrice { get; init; }
}

/// <summary>
/// Response contract returned when an order is successfully created (HTTP 201 Created).
/// 
/// Positional record — constructed by the service after successful persistence.
/// Contains the full order summary: system-assigned ID, timestamp, echoed customer
/// and currency, computed total, and item count.
/// A Location header pointing to /api/orders/{orderId} is set by the controller.
/// </summary>
/// <param name="OrderId">System-assigned order identifier (IDENTITY from Orders table).</param>
/// <param name="CreatedAtUtc">UTC timestamp set by the application at insert time.</param>
/// <param name="CustomerId">Customer ID echoed from the request for confirmation.</param>
/// <param name="Currency">Currency code echoed from the request.</param>
/// <param name="TotalAmount">Computed total: Σ(Quantity × UnitPrice) across all items.</param>
/// <param name="ItemsCount">Number of OrderItems rows inserted in this order.</param>
public record CreateOrderResponse(
    int OrderId,
    DateTime CreatedAtUtc,
    int CustomerId,
    string Currency,
    decimal TotalAmount,
    int ItemsCount);
