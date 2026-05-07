using OrdersApi.Contracts;
using OrdersApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace OrdersApi.Controllers;

/// <summary>
/// API controller for order operations.
/// Provides endpoints for creating and managing orders.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly OrderService _orderService;

    public OrdersController(OrderService orderService)
    {
        ArgumentNullException.ThrowIfNull(orderService);
        _orderService = orderService;
    }

    /// <summary>
    /// Creates a new order and calculates the total amount.
    ///
    /// The endpoint validates the order input, calculates the total using the
    /// business formula (subtotal, discount, tax), and returns the result.
    ///
    /// Formula: total = sum(Quantity * UnitPrice) * (1 - DiscountPercent/100) * (1 + TaxRate)
    /// Rounding: MidpointRounding.AwayFromZero to 2 decimals
    /// </summary>
    /// <param name="order">Order input containing items, discount, and tax rate.</param>
    /// <returns>
    /// 200 OK with the calculated order total if successful.
    /// 400 Bad Request if order validation fails.
    /// 500 Internal Server Error if calculation fails unexpectedly.
    /// </returns>
    [HttpPost]
    [ProducesResponseType(typeof(OrderTotalResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public IActionResult CreateOrder([FromBody] Order order)
    {
        if (order == null)
        {
            return BadRequest(new ProblemDetails
            {
                Title = "Invalid Order",
                Detail = "Order cannot be null."
            });
        }

        try
        {
            var total = _orderService.CalculateOrderTotal(order);

            return Ok(new OrderTotalResponse
            {
                OrderId = Guid.NewGuid(),
                Total = total,
                CalculatedAt = DateTime.UtcNow
            });
        }
        catch (ArgumentNullException ex)
        {
            return BadRequest(new ProblemDetails
            {
                Title = "Invalid Order",
                Detail = ex.Message
            });
        }
        catch (ArgumentOutOfRangeException ex)
        {
            return BadRequest(new ProblemDetails
            {
                Title = "Invalid Order Parameters",
                Detail = ex.Message
            });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new ProblemDetails
            {
                Title = "Invalid Order",
                Detail = ex.Message
            });
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new ProblemDetails
            {
                Title = "Order Calculation Failed",
                Detail = $"An unexpected error occurred: {ex.Message}"
            });
        }
    }
}

/// <summary>
/// Response model for order creation.
/// </summary>
public class OrderTotalResponse
{
    /// <summary>
    /// Unique identifier for the created order.
    /// </summary>
    public Guid OrderId { get; set; }

    /// <summary>
    /// Final calculated total amount.
    /// </summary>
    public decimal Total { get; set; }

    /// <summary>
    /// Timestamp of the calculation.
    /// </summary>
    public DateTime CalculatedAt { get; set; }
}
