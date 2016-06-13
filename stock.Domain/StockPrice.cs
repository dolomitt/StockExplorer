using System;

namespace stock.Domain
{
    public class StockPrice
    {
        public DateTime Date;
        public decimal Price;

        public override string ToString()
        {
            return "Date: " + Date + " - Price: " + this.Price;
        }
    }
}