using FluentAssertions;
using Moq;
using OrdersApi.Contracts;
using OrdersApi.Services;

namespace OrdersApi.Tests;

/// <summary>
/// Tests for CalculateOrderTotal() calculation correctness:
/// happy path, boundary values, rounding, and precision scenarios.
/// </summary>
public class OrderServiceCalculationTests
{
    private readonly Mock<IOrderRepository> _mockRepository;
    private readonly OrderService _service;

    public OrderServiceCalculationTests()
    {
        _mockRepository = new Mock<IOrderRepository>();
        _mockRepository.Setup(r => r.GetById(It.IsAny<int>()))
            .ReturnsAsync((Order?)null);
        _service = new OrderService(_mockRepository.Object);
    }

    private static Order CreateOrder(decimal unitPrice = 100m, int quantity = 1,
        decimal discount = 0m, decimal tax = 0m)
        => new OrderBuilder()
            .WithSingleItem(unitPrice, quantity)
            .WithDiscount(discount)
            .WithTax(tax)
            .Build();

    [Theory]
    [InlineData(0, 0, 100.00)]     // No discount, no tax
    [InlineData(10, 0, 90.00)]     // Discount applied
    [InlineData(0, 0.22, 122.00)]  // Tax applied
    [InlineData(10, 0.22, 109.80)] // Discount applied before tax
    public void CalculateOrderTotal_SingleItem_CalculatesCorrectly(
        decimal discount, decimal tax, decimal expected)
    {
        var order = CreateOrder(discount: discount, tax: tax);

        _service.CalculateOrderTotal(order).Should().Be(expected);
    }

    [Theory]
    [InlineData(0, 0, 100.00)]  // Discount boundary: 0%
    [InlineData(100, 0, 0.00)]  // Discount boundary: 100%
    public void CalculateOrderTotal_DiscountAtBoundaries_CalculatesCorrectly(
        decimal discount, decimal tax, decimal expected)
    {
        var order = CreateOrder(discount: discount, tax: tax);

        _service.CalculateOrderTotal(order).Should().Be(expected);
    }

    [Theory]
    [InlineData(10.004, 10.00)] // Below midpoint: rounds down
    [InlineData(10.005, 10.01)] // At midpoint: rounds away from zero
    [InlineData(10.006, 10.01)] // Above midpoint: rounds up
    public void CalculateOrderTotal_Rounding_UsesAwayFromZero(
        decimal unitPrice, decimal expected)
    {
        var order = CreateOrder(unitPrice: unitPrice);

        _service.CalculateOrderTotal(order).Should().Be(expected);
    }

    [Theory]
    [InlineData(1000, 0.01, 10.00)]   // 1000 items @ $0.01
    [InlineData(10000, 0.01, 100.00)] // 10000 items @ $0.01
    public void CalculateOrderTotal_HighQuantityLowPrice_MaintainsPrecision(
        int quantity, decimal unitPrice, decimal expected)
    {
        var order = CreateOrder(unitPrice: unitPrice, quantity: quantity);

        _service.CalculateOrderTotal(order).Should().Be(expected);
    }

    [Theory]
    [InlineData(15.5, 0, 84.50)]     // Fractional discount (15.5%)
    [InlineData(0, 0.065, 106.50)]   // Fractional tax rate (6.5%)
    [InlineData(0, 1.5, 250.00)]     // Tax exceeds 100% (150%)
    [InlineData(95, 0.01, 5.05)]     // High discount + low tax
    [InlineData(15.5, 0.065, 89.99)] // Fractional discount + tax (100 * 0.845 * 1.065 = 89.993 → 89.99)
    public void CalculateOrderTotal_FractionalAndExtremeValues_CalculatesCorrectly(
        decimal discount, decimal tax, decimal expected)
    {
        var order = CreateOrder(discount: discount, tax: tax);

        _service.CalculateOrderTotal(order).Should().Be(expected);
    }

    [Fact]
    public void CalculateOrderTotal_MultipleItems_ReturnsSummedTotal()
    {
        var order = new OrderBuilder()
            .WithItems(
                new OrderItem { Quantity = 2, UnitPrice = 50m },
                new OrderItem { Quantity = 3, UnitPrice = 40m },
                new OrderItem { Quantity = 1, UnitPrice = 100m }
            )
            .Build();

        // (2*50) + (3*40) + (1*100) = 100 + 120 + 100 = 320
        _service.CalculateOrderTotal(order).Should().Be(320.00m);
    }

    [Fact]
    public void CalculateOrderTotal_HundredItems_SumsAllCorrectly()
    {
        var items = Enumerable.Range(0, 100)
            .Select(_ => new OrderItem { Quantity = 1, UnitPrice = 10m })
            .ToArray();
        var order = new OrderBuilder().WithItems(items).Build();

        _service.CalculateOrderTotal(order).Should().Be(1000.00m);
    }

    [Fact]
    public void CalculateOrderTotal_ComplexRoundingWithDiscountAndTax_RoundsCorrectly()
    {
        // Subtotal: 23 → after 33.33% discount: 15.3341 → after 5.5% tax: 16.177... → 16.18
        var order = new OrderBuilder()
            .WithItems(
                new OrderItem { Quantity = 1, UnitPrice = 10m },
                new OrderItem { Quantity = 1, UnitPrice = 13m }
            )
            .WithDiscount(33.33m)
            .WithTax(0.055m)
            .Build();

        _service.CalculateOrderTotal(order).Should().Be(16.18m);
    }
}

/// <summary>
/// Tests for CalculateOrderTotal() input validation:
/// null inputs, empty collections, and out-of-range values.
/// </summary>
public class OrderServiceValidationTests
{
    private readonly Mock<IOrderRepository> _mockRepository;
    private readonly OrderService _service;

    public OrderServiceValidationTests()
    {
        _mockRepository = new Mock<IOrderRepository>();
        _mockRepository.Setup(r => r.GetById(It.IsAny<int>()))
            .ReturnsAsync((Order?)null);
        _service = new OrderService(_mockRepository.Object);
    }

    private static Order CreateOrder(decimal unitPrice = 100m, int quantity = 1,
        decimal discount = 0m, decimal tax = 0m)
        => new OrderBuilder()
            .WithSingleItem(unitPrice, quantity)
            .WithDiscount(discount)
            .WithTax(tax)
            .Build();

    [Theory]
    [InlineData(0, 100)]  // Zero quantity
    [InlineData(-5, 100)] // Negative quantity
    public void CalculateOrderTotal_InvalidQuantity_ThrowsArgumentException(
        int quantity, decimal unitPrice)
    {
        var order = CreateOrder(unitPrice: unitPrice, quantity: quantity);

        FluentActions.Invoking(() => _service.CalculateOrderTotal(order))
            .Should().Throw<ArgumentException>()
            .WithMessage("*Quantity > 0*");
    }

    [Theory]
    [InlineData(1, 0)]   // Zero price
    [InlineData(1, -10)] // Negative price
    public void CalculateOrderTotal_InvalidUnitPrice_ThrowsArgumentException(
        int quantity, decimal unitPrice)
    {
        var order = CreateOrder(unitPrice: unitPrice, quantity: quantity);

        FluentActions.Invoking(() => _service.CalculateOrderTotal(order))
            .Should().Throw<ArgumentException>()
            .WithMessage("*UnitPrice > 0*");
    }

    [Theory]
    [InlineData(-1)]  // Negative discount
    [InlineData(101)] // Discount exceeds 100%
    public void CalculateOrderTotal_DiscountOutOfRange_ThrowsArgumentOutOfRangeException(
        decimal discount)
    {
        var order = CreateOrder(discount: discount);

        FluentActions.Invoking(() => _service.CalculateOrderTotal(order))
            .Should().Throw<ArgumentOutOfRangeException>()
            .WithMessage("*0 and 100*");
    }

    [Theory]
    [InlineData(0, 40, "Quantity > 0")]  // Invalid quantity in middle
    [InlineData(3, 0, "UnitPrice > 0")] // Invalid price in middle
    public void CalculateOrderTotal_InvalidItemInMiddleOfCollection_ThrowsArgumentException(
        int middleQty, decimal middlePrice, string expectedMessagePart)
    {
        var order = new OrderBuilder()
            .WithItems(
                new OrderItem { Quantity = 2, UnitPrice = 50m },
                new OrderItem { Quantity = middleQty, UnitPrice = middlePrice },
                new OrderItem { Quantity = 1, UnitPrice = 100m }
            )
            .Build();

        FluentActions.Invoking(() => _service.CalculateOrderTotal(order))
            .Should().Throw<ArgumentException>()
            .WithMessage($"*{expectedMessagePart}*");
    }

    [Fact]
    public void CalculateOrderTotal_NullOrder_ThrowsArgumentNullException()
    {
        FluentActions.Invoking(() => _service.CalculateOrderTotal(null!))
            .Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void CalculateOrderTotal_NullItemsCollection_ThrowsArgumentException()
    {
        var order = new Order { Items = null!, DiscountPercent = 0m, TaxRate = 0m };

        FluentActions.Invoking(() => _service.CalculateOrderTotal(order))
            .Should().Throw<ArgumentException>()
            .WithMessage("*at least one item*");
    }

    [Fact]
    public void CalculateOrderTotal_EmptyItemsCollection_ThrowsArgumentException()
    {
        var order = new OrderBuilder().WithItems().Build();

        FluentActions.Invoking(() => _service.CalculateOrderTotal(order))
            .Should().Throw<ArgumentException>()
            .WithMessage("*at least one item*");
    }

    [Fact]
    public void CalculateOrderTotal_NegativeTaxRate_ThrowsArgumentOutOfRangeException()
    {
        var order = CreateOrder(tax: -0.05m);

        FluentActions.Invoking(() => _service.CalculateOrderTotal(order))
            .Should().Throw<ArgumentOutOfRangeException>()
            .WithMessage("*negative*");
    }

    [Fact]
    public void CalculateOrderTotal_NullItemInCollection_ThrowsArgumentException()
    {
        var order = new Order { Items = new OrderItem[] { null! }, DiscountPercent = 0m, TaxRate = 0m };

        FluentActions.Invoking(() => _service.CalculateOrderTotal(order))
            .Should().Throw<ArgumentException>()
            .WithMessage("*null item*");
    }
}
