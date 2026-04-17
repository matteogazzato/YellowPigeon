using OrdersApi.Contracts;

namespace OrdersApi.Services;

/// <summary>
/// Provides order-related business operations.
/// </summary>
public class OrderService
{
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
}
