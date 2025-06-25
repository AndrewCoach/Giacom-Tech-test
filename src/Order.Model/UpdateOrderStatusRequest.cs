using System.ComponentModel.DataAnnotations;

namespace Order.Model
{
    /// <summary>
    /// Represents the request body for updating an order's status.
    /// </summary>
    public class UpdateOrderStatusRequest
    {
        [Required]
        [MaxLength(20)]
        public string StatusName { get; set; }
    }
}
