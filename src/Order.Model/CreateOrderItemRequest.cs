using System;
using System.ComponentModel.DataAnnotations;

namespace Order.Model
{
    /// <summary>
    /// Represents a single item within a new order request.
    /// </summary>
    public class CreateOrderItemRequest
    {
        /// <summary>
        /// The unique identifier for the product being ordered.
        /// </summary>
        [Required]
        public Guid ProductId { get; set; }

        /// <summary>
        /// The number of units of the product being ordered. Must be at least 1.
        /// </summary>
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1.")]
        public int Quantity { get; set; }
    }
}