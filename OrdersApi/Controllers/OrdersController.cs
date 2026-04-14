using Microsoft.AspNetCore.Mvc;
using OrdersApi.Contracts;
using OrdersApi.Exceptions;
using OrdersApi.Services;

namespace OrdersApi.Controllers;

/// <summary>
/// POST /api/orders — Create a new order with items.
///
/// Maps service outcomes to HTTP responses:
///   201 Created         → success (with Location header)
///   400 Bad Request     → OrderValidationException (AC2 field errors)
///   409 Conflict        → OrderPersistenceException.IsConflict (FK/CHECK violation)
///   500 Internal Error  → unexpected exceptions (no stack trace in body)
/// </summary>
[ApiController]
[Route("api/orders")]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _orderService;
    private readonly ILogger<OrdersController> _logger;

    public OrdersController(IOrderService orderService, ILogger<OrdersController> logger)
    {
        _orderService = orderService;
        _logger = logger;
    }

    [HttpPost]
    [ProducesResponseType(typeof(CreateOrderResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateOrder(
        [FromBody] CreateOrderRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var response = await _orderService.CreateOrderAsync(request, cancellationToken);
            return Created($"/api/orders/{response.OrderId}", response);
        }
        catch (OrderValidationException ex)
        {
            return BadRequest(new { errors = ex.Errors });
        }
        catch (OrderPersistenceException ex) when (ex.IsConflict)
        {
            _logger.LogWarning("Order creation conflict. Detail: {Detail}", ex.Detail);
            return Conflict(new { error = "The request conflicts with existing data or database constraints." });
        }
        catch (OrderPersistenceException ex)
        {
            _logger.LogError(ex, "Database error during order creation. Detail: {Detail}", ex.Detail);
            return StatusCode(StatusCodes.Status500InternalServerError,
                new { error = "An unexpected error occurred. Please try again later." });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error in OrdersController.CreateOrder");
            return StatusCode(StatusCodes.Status500InternalServerError,
                new { error = "An unexpected error occurred. Please try again later." });
        }
    }
}
