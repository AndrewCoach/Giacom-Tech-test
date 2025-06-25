using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Order.Model
{
    /// <summary>
    /// Represents the request payload for creating a new order.
    /// </summary>
    public class CreateOrderRequest
    {
        /// <summary>
        /// The ID of the reseller placing the order.
        /// </summary>
        [Required]
        public Guid ResellerId { get; set; }

        /// <summary>
        /// The ID of the end-customer for whom the order is being placed.
        /// </summary>
        [Required]
        public Guid CustomerId { get; set; }

        /// <summary>
        /// A list of items to be included in the order. Must contain at least one item.
        /// </summary>
        [Required]
        [MinLength(1, ErrorMessage = "Order must contain at least one item.")]
        public List<CreateOrderItemRequest> Items { get; set; }
    }
}