using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Order.Model;
using Order.Service;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OrderService.WebAPI.Controllers
{
    [ApiController]
    [Route("orders")]
    [Produces("application/json")]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        /// <summary>
        /// Creates a new order.
        /// </summary>
        /// <param name="request">The details of the order to create.</param>
        /// <returns>The newly created order's details.</returns>
        [HttpPost]
        [ProducesResponseType(typeof(OrderDetail), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] CreateOrderRequest request)
        {
            try
            {
                var newOrder = await _orderService.CreateOrderAsync(request);

                if (newOrder == null)
                {
                    // This case might occur if creation fails for an unknown reason without an exception.
                    return BadRequest("Could not create the order.");
                }

                // A 201 Created response with Location header pointing to new resource.
                return CreatedAtAction(nameof(GetOrderById), new { orderId = newOrder.Id }, newOrder);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Retrieves a list of all orders, optionally filtered by status.
        /// </summary>
        /// <param name="status">Optional. The name of the status to filter by (e.g., 'Completed', 'Failed').</param>
        /// <returns>A list of order summaries.</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Get([FromQuery] string status)
        {
            var orders = await _orderService.GetOrdersAsync(status);
            return Ok(orders);
        }

        /// <summary>
        /// Retrieves the details of a specific order by its ID.
        /// </summary>
        /// <param name="orderId">The unique identifier of the order.</param>
        /// <returns>The detailed order information.</returns>
        [HttpGet("{orderId}", Name = "GetOrderById")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetOrderById(Guid orderId)
        {
            var order = await _orderService.GetOrderByIdAsync(orderId);
            if (order != null)
            {
                return Ok(order);
            }
            else
            {
                return NotFound();
            }
        }

        /// <summary>
        /// Updates the status of an existing order.
        /// </summary>
        /// <param name="orderId">The ID of the order to update.</param>
        /// <param name="request">The request containing the new status name.</param>
        /// <returns>No content if successful.</returns>
        [HttpPut("{orderId}/status")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateStatus(Guid orderId, [FromBody] UpdateOrderStatusRequest request)
        {
            var success = await _orderService.UpdateOrderStatusAsync(orderId, request.StatusName);

            if (success)
            {
                // 204 for success with no data
                return NoContent();
            }
            else
            {
                return NotFound($"Order with ID '{orderId}' or status '{request.StatusName}' not found.");
            }
        }

        /// <summary>
        /// Calculates the total profit for 'Completed' orders, grouped by year and month.
        /// </summary>
        /// <returns>A list of monthly profit summaries.</returns>
        [HttpGet("profit/monthly")]
        [ProducesResponseType(typeof(IEnumerable<MonthlyProfit>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetMonthlyProfit()
        {
            var profitReport = await _orderService.GetMonthlyProfitReportAsync();
            return Ok(profitReport);
        }
    }
}
