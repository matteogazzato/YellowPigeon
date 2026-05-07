using OrdersApi.Contracts;

namespace OrdersApi.Services;

/// <summary>
/// Provides order-related business operations.
/// </summary>
public class OrderService
{
    private readonly IOrderRepository _repository;

    /// <summary>
    /// Initializes a new instance of the OrderService.
    /// </summary>
    /// <param name="repository">The order repository for data access.</param>
    public OrderService(IOrderRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }
    /// <summary>
    /// Calculates the final order total using subtotal, discount, and tax.
    ///
    /// Formula:
    /// total = sum(Quantity * UnitPrice) * (1 - DiscountPercent/100) * (1 + TaxRate)
    ///
    /// Rounding:
    /// The final total is rounded to 2 decimals using MidpointRounding.AwayFromZero.
    ///
    /// Validation rules:
    /// - order must not be null
    /// - items collection must contain at least one item
    /// - each item must have Quantity > 0 and UnitPrice > 0
    /// - DiscountPercent must be in [0, 100]
    /// - TaxRate must be >= 0
    /// </summary>
    /// <param name="order">Order input used for total computation.</param>
    /// <returns>The final rounded monetary amount for the order.</returns>
    /// <exception cref="ArgumentNullException">Thrown when order is null.</exception>
    /// <exception cref="ArgumentException">Thrown when order or items contain invalid values.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when discount or tax values are outside valid ranges.</exception>
    public decimal CalculateOrderTotal(Order order)
    {
        ArgumentNullException.ThrowIfNull(order);

        if (order.Items is null || order.Items.Count == 0)
        {
            throw new ArgumentException("Order must contain at least one item.", nameof(order));
        }

        if (order.DiscountPercent < 0m || order.DiscountPercent > 100m)
        {
            throw new ArgumentOutOfRangeException(
                nameof(order),
                "DiscountPercent must be between 0 and 100.");
        }

        if (order.TaxRate < 0m)
        {
            throw new ArgumentOutOfRangeException(
                nameof(order),
                "TaxRate cannot be negative.");
        }

        decimal subtotal = 0m;

        foreach (var item in order.Items)
        {
            if (item is null)
            {
                throw new ArgumentException("Order contains a null item.", nameof(order));
            }

            if (item.Quantity <= 0)
            {
                throw new ArgumentException("Each item must have Quantity > 0.", nameof(order));
            }

            if (item.UnitPrice <= 0m)
            {
                throw new ArgumentException("Each item must have UnitPrice > 0.", nameof(order));
            }

            subtotal += item.Quantity * item.UnitPrice;
        }

        var discountMultiplier = 1m - (order.DiscountPercent / 100m);
        var taxMultiplier = 1m + order.TaxRate;

        var total = subtotal * discountMultiplier * taxMultiplier;

        return Math.Round(total, 2, MidpointRounding.AwayFromZero);
    }

    /// <summary>
    /// Applies a progressive bulk discount based on total item quantity.
    ///
    /// Discount tiers:
    /// - 1–10 items: 0% additional discount
    /// - 11–25 items: 5% additional discount
    /// - 26–50 items: 10% additional discount
    /// - 51+ items: 15% additional discount
    ///
    /// The bulk discount is returned as a decimal percentage (e.g., 5m for 5%).
    /// The caller is responsible for combining this with existing discounts
    /// and calculating the final order total.
    ///
    /// Validation rules:
    /// - order must not be null
    /// - items collection must contain at least one item
    /// - each item must have Quantity > 0
    /// </summary>
    /// <param name="order">Order used to determine total item quantity.</param>
    /// <returns>The bulk discount percentage to apply (e.g., 5m, 10m, 15m).</returns>
    /// <exception cref="ArgumentNullException">Thrown when order is null.</exception>
    /// <exception cref="ArgumentException">Thrown when items are null, empty, or contain invalid quantities.</exception>
    public decimal ApplyBulkDiscount(Order order)
    {
        ArgumentNullException.ThrowIfNull(order);

        if (order.Items is null || order.Items.Count == 0)
        {
            throw new ArgumentException("Order must contain at least one item.", nameof(order));
        }

        int totalQuantity = 0;

        foreach (var item in order.Items)
        {
            if (item is null)
            {
                throw new ArgumentException("Order contains a null item.", nameof(order));
            }

            if (item.Quantity <= 0)
            {
                throw new ArgumentException("Each item must have Quantity > 0.", nameof(order));
            }

            totalQuantity += item.Quantity;
        }

        return totalQuantity switch
        {
            >= 1 and <= 10 => 0m,
            >= 11 and <= 25 => 5m,
            >= 26 and <= 50 => 10m,
            >= 51 => 15m,
            _ => 0m // Should not reach here due to validation above
        };
    }
}
