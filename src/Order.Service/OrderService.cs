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

        public async Task<OrderDetail> CreateOrderAsync(CreateOrderRequest createOrderRequest)
        {
            // TODO: add more business logic in real scenario.
            // such as checking reseller credit limits or triggering notifications.
            try
            {
                return await _orderRepository.CreateOrderAsync(createOrderRequest);
            }
            catch (ArgumentException)
            {
                // If the repository throws a specific, known exception (like for invalid products),
                // re-throw it to be handled by the controller.
                throw;
            }
            // Other unexpected exceptions bubble up naturally.
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

        public async Task<IEnumerable<MonthlyProfit>> GetMonthlyProfitReportAsync()
        {
            return await _orderRepository.GetMonthlyProfitReportAsync();
        }
    }
}
