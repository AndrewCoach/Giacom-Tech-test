using NUnit.Framework;
using Order.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Order.Service.Tests
{
    public partial class OrderServiceTests
    {
        [Test]
        public async Task CreateOrderAsync_WithValidData_ShouldCreateOrderAndReturnDetails()
        {
            // Arrange
            var request = new CreateOrderRequest
            {
                ResellerId = Guid.NewGuid(),
                CustomerId = Guid.NewGuid(),
                Items = new List<CreateOrderItemRequest>
                {
                    new CreateOrderItemRequest { ProductId = new Guid(_orderProductEmailId), Quantity = 2 }
                }
            };

            // Act
            var createdOrder = await _orderService.CreateOrderAsync(request);

            // Assert
            Assert.IsNotNull(createdOrder);
            Assert.AreEqual(request.ResellerId, createdOrder.ResellerId);
            Assert.AreEqual(request.CustomerId, createdOrder.CustomerId);
            Assert.AreEqual("Created", createdOrder.StatusName);
            Assert.AreEqual(1, createdOrder.Items.Count());

            var createdItem = createdOrder.Items.First();
            Assert.AreEqual(new Guid(_orderProductEmailId), createdItem.ProductId);
            Assert.AreEqual(2, createdItem.Quantity);
            Assert.AreEqual(1.6m, createdOrder.TotalCost); // 2 * 0.8
            Assert.AreEqual(1.8m, createdOrder.TotalPrice); // 2 * 0.9
        }

        [Test]
        public void CreateOrderAsync_WithInvalidProductId_ShouldThrowArgumentException()
        {
            // Arrange
            var invalidProductId = Guid.NewGuid();
            var request = new CreateOrderRequest
            {
                ResellerId = Guid.NewGuid(),
                CustomerId = Guid.NewGuid(),
                Items = new List<CreateOrderItemRequest>
                {
                    new CreateOrderItemRequest { ProductId = invalidProductId, Quantity = 1 }
                }
            };

            // Act & Assert
            var ex = Assert.ThrowsAsync<ArgumentException>(async () => await _orderService.CreateOrderAsync(request));
            Assert.That(ex.Message, Does.Contain($"Invalid ProductId(s) provided: {invalidProductId}"));
        }

        [Test]
        public async Task CreateOrderAsync_WithMultipleItems_ShouldCalculateTotalsCorrectly()
        {
            // Arrange - add one product for this test to reference data
            var secondProductId = await AddSecondTestProduct();

            var request = new CreateOrderRequest
            {
                ResellerId = Guid.NewGuid(),
                CustomerId = Guid.NewGuid(),
                Items = new List<CreateOrderItemRequest>
                {
                    new CreateOrderItemRequest { ProductId = new Guid(_orderProductEmailId), Quantity = 1 }, // Cost 0.8, Price 0.9
                    new CreateOrderItemRequest { ProductId = secondProductId, Quantity = 2 } // Cost 2.0, Price 2.3
                }
            };

            // Expected totals:
            // Cost = (1 * 0.8) + (2 * 1.0) = 0.8 + 2.0 = 2.8
            // Price = (1 * 0.9) + (2 * 1.15) = 0.9 + 2.3 = 3.2
            var expectedTotalCost = 2.8m;
            var expectedTotalPrice = 3.2m;

            // Act
            var createdOrder = await _orderService.CreateOrderAsync(request);

            // Assert
            Assert.IsNotNull(createdOrder);
            Assert.AreEqual(2, createdOrder.Items.Count());
            Assert.AreEqual(expectedTotalCost, createdOrder.TotalCost);
            Assert.AreEqual(expectedTotalPrice, createdOrder.TotalPrice);
        }

        // Helper method to add another product for testing
        private async Task<Guid> AddSecondTestProduct()
        {
            var productId = Guid.NewGuid();
            _orderContext.OrderProduct.Add(new Data.Entities.OrderProduct
            {
                Id = productId.ToByteArray(),
                Name = "Test Product 2",
                ServiceId = _orderServiceEmailId,
                UnitCost = 1.0m,
                UnitPrice = 1.15m
            });
            await _orderContext.SaveChangesAsync();
            return productId;
        }
    }
}