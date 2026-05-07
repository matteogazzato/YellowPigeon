using OrdersApi.Contracts;

namespace OrdersApi.Tests;

/// <summary>
/// Test builder for fluent Order construction in unit tests.
/// Provides sensible defaults and chainable methods for readable test setup.
/// </summary>
public class OrderBuilder
{
    private decimal _discountPercent = 0m;
    private decimal _taxRate = 0m;
    private readonly List<OrderItem> _items = [];

    /// <summary>
    /// Adds a single item to the order.
    /// </summary>
    public OrderBuilder WithItem(int quantity, decimal unitPrice)
    {
        _items.Add(new OrderItem { Quantity = quantity, UnitPrice = unitPrice });
        return this;
    }

    /// <summary>
    /// Replaces all items with a single item of the specified quantity and price.
    /// Useful for most test scenarios with a single product.
    /// </summary>
    public OrderBuilder WithSingleItem(decimal unitPrice, int quantity = 1)
    {
        _items.Clear();
        _items.Add(new OrderItem { Quantity = quantity, UnitPrice = unitPrice });
        return this;
    }

    /// <summary>
    /// Sets the discount percentage for the order (0-100).
    /// </summary>
    public OrderBuilder WithDiscount(decimal discountPercent)
    {
        _discountPercent = discountPercent;
        return this;
    }

    /// <summary>
    /// Sets the tax rate for the order.
    /// </summary>
    public OrderBuilder WithTax(decimal taxRate)
    {
        _taxRate = taxRate;
        return this;
    }

    /// <summary>
    /// Clears all items and sets custom items collection.
    /// Useful when you need precise control over the items list.
    /// </summary>
    public OrderBuilder WithItems(params OrderItem[] items)
    {
        _items.Clear();
        _items.AddRange(items);
        return this;
    }

    /// <summary>
    /// Builds and returns the configured Order instance.
    /// </summary>
    public Order Build() =>
        new()
        {
            Items = _items,
            DiscountPercent = _discountPercent,
            TaxRate = _taxRate
        };
}
