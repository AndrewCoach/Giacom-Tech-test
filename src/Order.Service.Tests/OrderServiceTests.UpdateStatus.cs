using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace Order.Service.Tests
{
    public partial class OrderServiceTests
    {
        [Test]
        public async Task UpdateOrderStatusAsync_WithValidIdAndStatus_ShouldUpdateStatus()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            await AddOrder(orderId, 1, "Created");

            // Act
            var result = await _orderService.UpdateOrderStatusAsync(orderId, "Completed");
            var updatedOrder = await _orderService.GetOrderByIdAsync(orderId);

            // Assert
            Assert.IsTrue(result, "Update operation should return true for success.");
            Assert.AreEqual("Completed", updatedOrder.StatusName, "Order status should be updated to 'Completed'.");
        }

        [Test]
        public async Task UpdateOrderStatusAsync_WithInvalidOrderId_ShouldReturnFalse()
        {
            // Arrange
            var nonExistentOrderId = Guid.NewGuid();

            // Act
            var result = await _orderService.UpdateOrderStatusAsync(nonExistentOrderId, "Completed");

            // Assert
            Assert.IsFalse(result, "Update operation should return false for a non-existent order ID.");
        }

        [Test]
        public async Task UpdateOrderStatusAsync_WithInvalidStatusName_ShouldReturnFalse()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            await AddOrder(orderId, 1, "Created");
            var invalidStatus = "NonExistentStatus";

            // Act
            var result = await _orderService.UpdateOrderStatusAsync(orderId, invalidStatus);
            var order = await _orderService.GetOrderByIdAsync(orderId);


            // Assert
            Assert.IsFalse(result, "Update operation should return false for a non-existent status name.");
            Assert.AreEqual("Created", order.StatusName, "Order status should not have changed.");
        }
    }
}
