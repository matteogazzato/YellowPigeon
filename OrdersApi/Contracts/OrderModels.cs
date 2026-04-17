namespace OrdersApi.Contracts;

/// <summary>
/// Represents an order used by the business logic to compute totals.
///
/// The total amount is derived from item lines and pricing rules,
/// such as discount and tax values.
/// </summary>
public class Order
{
    /// <summary>
    /// Collection of order lines to be used in total calculation.
    /// </summary>
    public IReadOnlyCollection<OrderItem> Items { get; init; } = Array.Empty<OrderItem>();

    /// <summary>
    /// Discount percentage applied to the subtotal.
    /// Expected range: 0 to 100.
    /// </summary>
    public decimal DiscountPercent { get; init; }

    /// <summary>
    /// Tax rate applied after discount.
    /// Example: 0.22 means 22% tax.
    /// </summary>
    public decimal TaxRate { get; init; }
}

/// <summary>
/// Represents a single order line.
/// </summary>
public class OrderItem
{
    /// <summary>
    /// Quantity for the line item.
    /// </summary>
    public int Quantity { get; init; }

    /// <summary>
    /// Unit price for the line item.
    /// </summary>
    public decimal UnitPrice { get; init; }
}
