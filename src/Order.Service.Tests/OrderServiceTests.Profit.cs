using NUnit.Framework;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Order.Service.Tests
{
    public partial class OrderServiceTests
    {
        [Test]
        public async Task GetMonthlyProfitReportAsync_ShouldReturnCorrectlyGroupedAndSummedProfit()
        {
            // Arrange
            // Our test product has UnitCost = 0.8 and UnitPrice = 0.9, so profit per item is 0.1.

            // January 2023: 2 completed orders, total profit = (1 * 0.1) + (2 * 0.1) = 0.3
            await AddOrder(Guid.NewGuid(), 1, "Completed", new DateTime(2023, 1, 10));
            await AddOrder(Guid.NewGuid(), 2, "Completed", new DateTime(2023, 1, 20));

            // February 2023: 1 completed order, total profit = (3 * 0.1) = 0.3
            await AddOrder(Guid.NewGuid(), 3, "Completed", new DateTime(2023, 2, 5));

            // January 2023: 1 'Created' order, should be ignored by the calculation
            await AddOrder(Guid.NewGuid(), 5, "Created", new DateTime(2023, 1, 15));

            // March 2023: No completed orders

            // April 2023: 1 completed order, total profit = (4 * 0.1) = 0.4
            await AddOrder(Guid.NewGuid(), 4, "Completed", new DateTime(2023, 4, 1));


            // Act
            var profitReport = await _orderService.GetMonthlyProfitReportAsync();
            var profitReportList = profitReport.ToList();

            // Assert
            Assert.AreEqual(3, profitReportList.Count, "Should return 3 groups, one fo reach month with completed orders.");

            var janProfit = profitReportList.FirstOrDefault(p => p.Year == 2023 && p.Month == 1);
            Assert.IsNotNull(janProfit, "Should be an entry for January 2023.");
            Assert.AreEqual(0.3m, janProfit.TotalProfit, "Profit for January 2023 is incorect.");

            var febProfit = profitReportList.FirstOrDefault(p => p.Year == 2023 && p.Month == 2);
            Assert.IsNotNull(febProfit, "Should be an entry for February 2023.");
            Assert.AreEqual(0.3m, febProfit.TotalProfit, "Profit for February 2023 is incorect.");

            var aprProfit = profitReportList.FirstOrDefault(p => p.Year == 2023 && p.Month == 4);
            Assert.IsNotNull(aprProfit, "Should be an entry for April 2023.");
            Assert.AreEqual(0.4m, aprProfit.TotalProfit, "Profit for April 2023 is incorect.");
        }
    }
}