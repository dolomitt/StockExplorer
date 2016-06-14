using System;
using stock.Domain;

namespace stock.Engine
{
    public class TransactionFactory : ITransactionFactory
    {
        public Transaction Create(TransactionType transType, long count, DateTime date, decimal price)
        {
            if (transType == TransactionType.Buy)
            {
                return new BuyTransaction()
                {
                    Count = count,
                    Price = price,
                    Date = date
                };
            }
            else if (transType == TransactionType.Sell)
            {
                return new SellTransaction()
                {
                    Count = count,
                    Date = date,
                    Price = price
                };
            }
            else
            {
                return null;
            }
        }
    }
}