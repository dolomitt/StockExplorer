using System;

namespace stock.DataLoader
{
    public class StockPriceData
    {
        public DateTime Date;
        public decimal Open;
        public decimal High;
        public decimal Low;
        public decimal Close;
        public decimal AdjClose;
        public long Volume;

        public override string ToString()
        {
            return "Date: " + Date.ToShortDateString() + " - Open: " + this.Open;
        }

        /// <summary>
        /// High minus Low
        /// </summary>
        public decimal DailySpread => (High - Low);

        public decimal DailyHigh => (High - Open);
    }
}
