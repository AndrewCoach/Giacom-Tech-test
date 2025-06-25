namespace Order.Model
{
    /// <summary>
    /// Represents the total profit for a specific month and year.
    /// </summary>
    public class MonthlyProfit
    {
        /// <summary>
        /// The year of the profit calculation.
        /// </summary>
        public int Year { get; set; }

        /// <summary>
        /// The month of the profit calculation (1-12).
        /// </summary>
        public int Month { get; set; }

        /// <summary>
        /// The total calculated profit for the month.
        /// Profit is defined as (TotalPrice - TotalCost).
        /// </summary>
        public decimal TotalProfit { get; set; }
    }
}