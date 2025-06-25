using Microsoft.EntityFrameworkCore;
using Order.Data.Entities;
using Order.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OrderItem = Order.Data.Entities.OrderItem;

namespace Order.Data
{
    public class OrderRepository : IOrderRepository
    {
        private readonly OrderContext _orderContext;

        public OrderRepository(OrderContext orderContext)
        {
            _orderContext = orderContext;
        }

        public async Task<IEnumerable<OrderSummary>> GetOrdersAsync(string status = null)
        {
            var query = _orderContext.Order
                .Include(x => x.Items)
                .Include(x => x.Status)
                .AsQueryable();

            // If a status is provided, add a WHERE clause to filter the results.
            // We compare the status name in a case-insensitive manner for robustness.
            if (!string.IsNullOrWhiteSpace(status))
            {
                query = query.Where(o => o.Status.Name.ToLower() == status.ToLower());
            }

            var orders = await query
                .Select(x => new OrderSummary
                {
                    Id = new Guid(x.Id),
                    ResellerId = new Guid(x.ResellerId),
                    CustomerId = new Guid(x.CustomerId),
                    StatusId = new Guid(x.StatusId),
                    StatusName = x.Status.Name,
                    ItemCount = x.Items.Count,
                    TotalCost = x.Items.Sum(i => i.Quantity * i.Product.UnitCost).Value,
                    TotalPrice = x.Items.Sum(i => i.Quantity * i.Product.UnitPrice).Value,
                    CreatedDate = x.CreatedDate
                })
                .OrderByDescending(x => x.CreatedDate)
                .ToListAsync();

            return orders;
        }

        public async Task<OrderDetail> GetOrderByIdAsync(Guid orderId)
        {
            var orderIdBytes = orderId.ToByteArray();

            var order = await _orderContext.Order
                .Where(x => _orderContext.Database.IsInMemory() ? x.Id.SequenceEqual(orderIdBytes) : x.Id == orderIdBytes)
                .Select(x => new OrderDetail
                {
                    Id = new Guid(x.Id),
                    ResellerId = new Guid(x.ResellerId),
                    CustomerId = new Guid(x.CustomerId),
                    StatusId = new Guid(x.StatusId),
                    StatusName = x.Status.Name,
                    CreatedDate = x.CreatedDate,
                    TotalCost = x.Items.Sum(i => i.Quantity * i.Product.UnitCost).Value,
                    TotalPrice = x.Items.Sum(i => i.Quantity * i.Product.UnitPrice).Value,
                    Items = x.Items.Select(i => new Model.OrderItem
                    {
                        Id = new Guid(i.Id),
                        OrderId = new Guid(i.OrderId),
                        ServiceId = new Guid(i.ServiceId),
                        ServiceName = i.Service.Name,
                        ProductId = new Guid(i.ProductId),
                        ProductName = i.Product.Name,
                        UnitCost = i.Product.UnitCost,
                        UnitPrice = i.Product.UnitPrice,
                        TotalCost = i.Product.UnitCost * i.Quantity.Value,
                        TotalPrice = i.Product.UnitPrice * i.Quantity.Value,
                        Quantity = i.Quantity.Value
                    })
                }).SingleOrDefaultAsync();

            return order;
        }

        public async Task<OrderDetail> CreateOrderAsync(CreateOrderRequest createOrderRequest)
        {
            // Fetch all requested product details in a single query for efficiency and validation.
            var productIds = createOrderRequest.Items.Select(i => i.ProductId.ToByteArray()).ToList();
            var products = await _orderContext.OrderProduct
                .Where(p => productIds.Contains(p.Id))
                .ToDictionaryAsync(p => new Guid(p.Id));

            // Validate that all product IDs provided in the request are valid.
            if (products.Count != createOrderRequest.Items.Select(i => i.ProductId).Distinct().Count())
            {
                // One or more product IDs were not found in the database.
                // We throw an exception here .
                var notFoundIds = createOrderRequest.Items.Select(i => i.ProductId).Where(id => !products.ContainsKey(id));
                throw new ArgumentException($"Invalid ProductId(s) provided: {string.Join(", ", notFoundIds)}");
            }

            // Every new order should start with a -Created status.
            var createdStatus = await _orderContext.OrderStatus
                .SingleAsync(s => s.Name == "Created");

            var newOrderId = Guid.NewGuid();
            var newOrderEntity = new Entities.Order
            {
                Id = newOrderId.ToByteArray(),
                ResellerId = createOrderRequest.ResellerId.ToByteArray(),
                CustomerId = createOrderRequest.CustomerId.ToByteArray(),
                StatusId = createdStatus.Id,
                CreatedDate = DateTime.UtcNow, // Use UTC for server-side timestamps.
                Items = new List<OrderItem>()
            };

            // Create the OrderItem entities from the request.
            foreach (var itemRequest in createOrderRequest.Items)
            {
                var product = products[itemRequest.ProductId];
                var orderItem = new OrderItem
                {
                    Id = Guid.NewGuid().ToByteArray(),
                    OrderId = newOrderEntity.Id,
                    ProductId = product.Id,
                    ServiceId = product.ServiceId, // Get the ServiceId from the product itself.
                    Quantity = itemRequest.Quantity
                };
                newOrderEntity.Items.Add(orderItem);
            }

            _orderContext.Order.Add(newOrderEntity);
            await _orderContext.SaveChangesAsync();

            // After successfully creating the order, fetch it's complete details
            // to return a full representation of the new resource.
            return await GetOrderByIdAsync(newOrderId);
        }

        public async Task<bool> UpdateOrderStatusAsync(Guid orderId, string newStatusName)
        {
            var newStatus = await _orderContext.OrderStatus
                .FirstOrDefaultAsync(s => s.Name.ToLower() == newStatusName.ToLower());

            // If the status name is invalid, do not proceed.
            if (newStatus == null)
            {
                return false;
            }

            var orderIdBytes = orderId.ToByteArray();

            var orderToUpdate = await _orderContext.Order
                .FirstOrDefaultAsync(o => o.Id == orderIdBytes);

            // If the order doesn't exist, do not proceed.
            if (orderToUpdate == null)
            {
                return false;
            }

            orderToUpdate.StatusId = newStatus.Id;
            await _orderContext.SaveChangesAsync();

            return true;
        }
    }
}