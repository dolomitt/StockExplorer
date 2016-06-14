using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using stock.Domain;

namespace stock.Strategy
{
    public class StandardStockStrategy : IStockTradingStrategy
    {
        private decimal highSellingThreshold;
        private decimal lowSellingThreshold;

        private Func<decimal,decimal> ComputeTransactionFee { get; set; }

        public StandardStockStrategy(Func<decimal, decimal> computeTransactionFee)
        {
            this.ComputeTransactionFee = computeTransactionFee;
        }

        public StandardStockStrategy(Func<decimal, decimal> computeTransactionFee, decimal lowSellingThreshold, decimal highSellingThreshold) : this(computeTransactionFee)
        {
            this.highSellingThreshold = highSellingThreshold;
            this.lowSellingThreshold = lowSellingThreshold;
        }


        public Transaction Process(ITransactionFactory factory, Vault vault, StockPrice stockPrice)
        {
            //No Current Transaction
            if (vault.CurrentTransaction == null)
            {
                var stockBuyCount = (long)Math.Floor(vault.Money / stockPrice.Price);
                var transaction = factory.Create(TransactionType.Buy, stockBuyCount, stockPrice.Date, stockPrice.Price);

                while (transaction.GetAmount() + ComputeTransactionFee(transaction.GetAmount()) > vault.Money)
                {
                    stockBuyCount--;
                    transaction = factory.Create(TransactionType.Buy, stockBuyCount, stockPrice.Date, stockPrice.Price);
                }

                return transaction;
            }
            else
            {
                var highThresholdPrice = vault.CurrentTransaction.Price * ((decimal)100 + this.highSellingThreshold) / (decimal)100;
                var lowThresholdPrice = vault.CurrentTransaction.Price * ((decimal)100 - this.lowSellingThreshold) / (decimal)100;

                var sellingPrice = decimal.MinusOne;

                if (stockPrice.Price > highThresholdPrice)
                    sellingPrice = highThresholdPrice;
                else if (stockPrice.Price < lowThresholdPrice)
                    sellingPrice = lowThresholdPrice;

                if (sellingPrice != decimal.MinusOne)
                {
                    var transaction = factory.Create(TransactionType.Sell, vault.CurrentTransaction.Count,
                        stockPrice.Date, sellingPrice);

                    return transaction;
                }
            }

            return null;
        }
    }
}
