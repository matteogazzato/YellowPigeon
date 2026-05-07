using Xunit;
using FluentAssertions;
using OrdersApi.Contracts;
using OrdersApi.Services;

namespace OrdersApi.Tests;

public class OrderServiceTests
{
    #region Baseline Tests - Normal Cases

    [Fact]
    public void CalculateOrderTotal_SingleItemNoDiscountNoTax_ShouldReturnItemPrice()
    {
        // Arrange
        var order = new Order
        {
            Items = new[] { new OrderItem { Quantity = 1, UnitPrice = 100m } },
            DiscountPercent = 0m,
            TaxRate = 0m
        };
        var service = new OrderService();

        // Act
        var result = service.CalculateOrderTotal(order);

        // Assert
        result.Should().Be(100m);
    }

    [Fact]
    public void CalculateOrderTotal_MultipleItemsNoDiscountNoTax_ShouldReturnSumOfItems()
    {
        // Arrange
        var order = new Order
        {
            Items = new[]
            {
                new OrderItem { Quantity = 2, UnitPrice = 50m },
                new OrderItem { Quantity = 3, UnitPrice = 30m }
            },
            DiscountPercent = 0m,
            TaxRate = 0m
        };
        var service = new OrderService();

        // Act
        var result = service.CalculateOrderTotal(order);

        // Assert
        // (2 * 50) + (3 * 30) = 100 + 90 = 190
        result.Should().Be(190m);
    }

    #endregion

    #region Discount Tests

    [Fact]
    public void CalculateOrderTotal_WithSimpleDiscount_ShouldApplyCorrectly()
    {
        // Arrange
        var order = new Order
        {
            Items = new[] { new OrderItem { Quantity = 1, UnitPrice = 100m } },
            DiscountPercent = 10m,
            TaxRate = 0m
        };
        var service = new OrderService();

        // Act
        var result = service.CalculateOrderTotal(order);

        // Assert
        // 100 * (1 - 10/100) = 100 * 0.9 = 90
        result.Should().Be(90m);
    }

    [Fact]
    public void CalculateOrderTotal_With50PercentDiscount_ShouldReturnHalfPrice()
    {
        // Arrange
        var order = new Order
        {
            Items = new[] { new OrderItem { Quantity = 1, UnitPrice = 200m } },
            DiscountPercent = 50m,
            TaxRate = 0m
        };
        var service = new OrderService();

        // Act
        var result = service.CalculateOrderTotal(order);

        // Assert
        // 200 * (1 - 50/100) = 200 * 0.5 = 100
        result.Should().Be(100m);
    }

    #endregion

    #region Tax Tests

    [Fact]
    public void CalculateOrderTotal_WithSimpleTax_ShouldApplyCorrectly()
    {
        // Arrange
        var order = new Order
        {
            Items = new[] { new OrderItem { Quantity = 1, UnitPrice = 100m } },
            DiscountPercent = 0m,
            TaxRate = 0.22m
        };
        var service = new OrderService();

        // Act
        var result = service.CalculateOrderTotal(order);

        // Assert
        // 100 * (1 + 0.22) = 100 * 1.22 = 122
        result.Should().Be(122m);
    }

    #endregion

    #region Combined Discount and Tax Tests

    [Fact]
    public void CalculateOrderTotal_WithDiscountAndTax_ShouldApplyInCorrectOrder()
    {
        // Arrange
        var order = new Order
        {
            Items = new[] { new OrderItem { Quantity = 1, UnitPrice = 100m } },
            DiscountPercent = 20m,
            TaxRate = 0.22m
        };
        var service = new OrderService();

        // Act
        var result = service.CalculateOrderTotal(order);

        // Assert
        // Formula: subtotal * (1 - discount/100) * (1 + tax)
        // 100 * (1 - 20/100) * (1 + 0.22)
        // 100 * 0.8 * 1.22
        // 80 * 1.22 = 97.6
        result.Should().Be(97.6m);
    }

    #endregion

    #region Edge Cases - Boundaries and Rounding

    [Fact]
    public void CalculateOrderTotal_WithRoundingNeeded_ShouldRoundAwayFromZero()
    {
        // Arrange
        var order = new Order
        {
            Items = new[] { new OrderItem { Quantity = 1, UnitPrice = 33.33m } },
            DiscountPercent = 0m,
            TaxRate = 0.22m
        };
        var service = new OrderService();

        // Act
        var result = service.CalculateOrderTotal(order);

        // Assert
        // 33.33 * 1.22 = 40.6626
        // MidpointRounding.AwayFromZero rounds to 40.66
        result.Should().Be(40.66m);
    }

    [Fact]
    public void CalculateOrderTotal_ZeroDiscount_ShouldBeSameAsNoDiscount()
    {
        // Arrange
        var order = new Order
        {
            Items = new[] { new OrderItem { Quantity = 1, UnitPrice = 100m } },
            DiscountPercent = 0m,
            TaxRate = 0m
        };
        var service = new OrderService();

        // Act
        var result = service.CalculateOrderTotal(order);

        // Assert
        result.Should().Be(100m);
    }

    [Fact]
    public void CalculateOrderTotal_MaxDiscount100Percent_ShouldReturnZero()
    {
        // Arrange
        var order = new Order
        {
            Items = new[] { new OrderItem { Quantity = 1, UnitPrice = 100m } },
            DiscountPercent = 100m,
            TaxRate = 0m
        };
        var service = new OrderService();

        // Act
        var result = service.CalculateOrderTotal(order);

        // Assert
        // 100 * (1 - 100/100) = 100 * 0 = 0
        result.Should().Be(0m);
    }

    #endregion

    #region Scenario: BUG REPRODUCTION - Bulk Discount with Multiple Items

    /// <summary>
    /// This test reproduces the bug scenario from Lab 3.
    /// 
    /// Scenario: An order with 3 items, each $100, and a 60% bulk discount
    /// should result in:
    /// Subtotal: 300
    /// After 60% discount: 300 * 0.4 = 120
    /// After 22% tax: 120 * 1.22 = 146.4
    /// 
    /// BUG: Due to incorrect discount calculation logic, the result is negative
    /// or incorrect. This test should FAIL initially.
    /// 
    /// ROOT CAUSE (to be discovered): 
    /// The CalculateOrderTotal() method has a bug in discount/tax application
    /// (introduced for debugging practice).
    /// </summary>
    [Fact]
    public void CalculateOrderTotal_WithBulkDiscountAnd3Items_ShouldReturnPositiveTotal()
    {
        // Arrange
        var order = new Order
        {
            Items = new[]
            {
                new OrderItem { Quantity = 1, UnitPrice = 100m },
                new OrderItem { Quantity = 1, UnitPrice = 100m },
                new OrderItem { Quantity = 1, UnitPrice = 100m }
            },
            DiscountPercent = 60m,
            TaxRate = 0.22m
        };
        var service = new OrderService();

        // Act
        var result = service.CalculateOrderTotal(order);

        // Assert - Expected calculation:
        // Subtotal: 300
        // After 60% discount: 300 * (1 - 60/100) = 300 * 0.4 = 120
        // After 22% tax: 120 * (1 + 0.22) = 120 * 1.22 = 146.4
        result.Should().Be(146.4m);
        result.Should().BeGreaterThan(0, "Total must always be positive");
    }

    #endregion

    #region Validation Tests

    [Fact]
    public void CalculateOrderTotal_NullOrder_ShouldThrowArgumentNullException()
    {
        // Arrange
        Order order = null;
        var service = new OrderService();

        // Act & Assert
        var exception = Assert.Throws<ArgumentNullException>(() => service.CalculateOrderTotal(order));
        exception.ParamName.Should().Be("order");
    }

    [Fact]
    public void CalculateOrderTotal_EmptyItems_ShouldThrowArgumentException()
    {
        // Arrange
        var order = new Order
        {
            Items = Array.Empty<OrderItem>(),
            DiscountPercent = 0m,
            TaxRate = 0m
        };
        var service = new OrderService();

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => service.CalculateOrderTotal(order));
        exception.ParamName.Should().Be("order");
        exception.Message.Should().Contain("at least one item");
    }

    [Fact]
    public void CalculateOrderTotal_NegativeDiscount_ShouldThrowArgumentOutOfRangeException()
    {
        // Arrange
        var order = new Order
        {
            Items = new[] { new OrderItem { Quantity = 1, UnitPrice = 100m } },
            DiscountPercent = -10m,
            TaxRate = 0m
        };
        var service = new OrderService();

        // Act & Assert
        var exception = Assert.Throws<ArgumentOutOfRangeException>(() => service.CalculateOrderTotal(order));
        exception.ParamName.Should().Be("order");
    }

    [Fact]
    public void CalculateOrderTotal_DiscountOver100Percent_ShouldThrowArgumentOutOfRangeException()
    {
        // Arrange
        var order = new Order
        {
            Items = new[] { new OrderItem { Quantity = 1, UnitPrice = 100m } },
            DiscountPercent = 120m,
            TaxRate = 0m
        };
        var service = new OrderService();

        // Act & Assert
        var exception = Assert.Throws<ArgumentOutOfRangeException>(() => service.CalculateOrderTotal(order));
        exception.ParamName.Should().Be("order");
    }

    [Fact]
    public void CalculateOrderTotal_NegativeTax_ShouldThrowArgumentOutOfRangeException()
    {
        // Arrange
        var order = new Order
        {
            Items = new[] { new OrderItem { Quantity = 1, UnitPrice = 100m } },
            DiscountPercent = 0m,
            TaxRate = -0.22m
        };
        var service = new OrderService();

        // Act & Assert
        var exception = Assert.Throws<ArgumentOutOfRangeException>(() => service.CalculateOrderTotal(order));
        exception.ParamName.Should().Be("order");
    }

    [Fact]
    public void CalculateOrderTotal_ItemWithZeroQuantity_ShouldThrowArgumentException()
    {
        // Arrange
        var order = new Order
        {
            Items = new[] { new OrderItem { Quantity = 0, UnitPrice = 100m } },
            DiscountPercent = 0m,
            TaxRate = 0m
        };
        var service = new OrderService();

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => service.CalculateOrderTotal(order));
        exception.ParamName.Should().Be("order");
        exception.Message.Should().Contain("Quantity > 0");
    }

    [Fact]
    public void CalculateOrderTotal_ItemWithZeroPrice_ShouldThrowArgumentException()
    {
        // Arrange
        var order = new Order
        {
            Items = new[] { new OrderItem { Quantity = 1, UnitPrice = 0m } },
            DiscountPercent = 0m,
            TaxRate = 0m
        };
        var service = new OrderService();

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => service.CalculateOrderTotal(order));
        exception.ParamName.Should().Be("order");
        exception.Message.Should().Contain("UnitPrice > 0");
    }

    #endregion

    #region Parameterized Tests - Multiple Scenarios

    [Theory]
    [InlineData(0, 0, 100)] // No discount, no tax
    [InlineData(10, 0, 90)] // 10% discount, no tax
    [InlineData(0, 0.22, 122)] // No discount, 22% tax
    [InlineData(20, 0.22, 97.6)] // 20% discount, 22% tax
    public void CalculateOrderTotal_VariousDiscountAndTaxCombinations_ShouldCalculateCorrectly(
        double discountPercent, double taxRate, double expected)
    {
        // Arrange
        var order = new Order
        {
            Items = new[] { new OrderItem { Quantity = 1, UnitPrice = 100m } },
            DiscountPercent = (decimal)discountPercent,
            TaxRate = (decimal)taxRate
        };
        var service = new OrderService();

        // Act
        var result = service.CalculateOrderTotal(order);

        // Assert
        result.Should().Be((decimal)expected);
    }

    #endregion
}
