using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Order.Model;
using Order.Service;
using System;
using System.Threading.Tasks;

namespace OrderService.WebAPI.Controllers
{
    [ApiController]
    [Route("orders")]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

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

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Get([FromQuery] string status)
        {
            var orders = await _orderService.GetOrdersAsync(status);
            return Ok(orders);
        }

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
    }
}
