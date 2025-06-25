using Order.Data;
using Order.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Order.Service
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;

        public OrderService(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public async Task<IEnumerable<OrderSummary>> GetOrdersAsync(string status = null)
        {
            return await _orderRepository.GetOrdersAsync(status);
        }

        public async Task<OrderDetail> GetOrderByIdAsync(Guid orderId)
        {
            return await _orderRepository.GetOrderByIdAsync(orderId);
        }

        public async Task<bool> UpdateOrderStatusAsync(Guid orderId, string newStatusName)
        {
            return await _orderRepository.UpdateOrderStatusAsync(orderId, newStatusName);
        }
    }
}
