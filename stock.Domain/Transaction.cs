using System;

namespace stock.Domain
{
    public class BuyTransaction : Transaction
    {
        protected override string GetTransactionType()
        {
            return "Buy";
        }
    }

    public class SellTransaction : Transaction
    {
        protected override string GetTransactionType()
        {
            return "Sell";
        }
    }

    public abstract class Transaction
    {
        public decimal Price;
        public long Count;
        public DateTime Date;
        public decimal BankFee;

        public Transaction LinkedTransaction { get; set; }

        /// <summary>
        /// Transaction Amount Excluding Bank Fees
        /// </summary>
        /// <returns></returns>
        public decimal GetAmount()
        {
            return Price * (decimal)Count;
        }

        public override string ToString()
        {
            return string.Format("{0}: {5} Count={1} - Price={2:C2} - Amount={3:C2} - BankFee={4:C2}", Date.ToShortDateString(), Count, Price, GetAmount(), BankFee, GetTransactionType());
        }

        protected abstract string GetTransactionType();
    }
}
