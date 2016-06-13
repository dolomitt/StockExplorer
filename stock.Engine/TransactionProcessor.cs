using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using stock.Domain;

namespace stock.Engine
{
    public class TransactionProcessor
    {
        private readonly decimal highSellingThreshold;
        private readonly decimal lowSellingThreshold;

        public Vault Vault => vault;
        private readonly Vault vault;

        public TransactionProcessor(decimal initialAmount, decimal highSellingThreshold, decimal lowSellingThreshold)
        {
            this.highSellingThreshold = highSellingThreshold;
            this.lowSellingThreshold = lowSellingThreshold;

            vault = new Vault(initialAmount, ComputeTransactionFee);
        }

        /// <summary>
        /// Compute Transaction Fee according to amount of buying or selling
        /// </summary>
        /// <param name="amount">Total Amount of Transaction</param>
        /// <returns>Bank Fee in Currency</returns>
        public static decimal ComputeTransactionFee(decimal amount)
        {
            if (amount <= 1000)
                return (decimal)2.50;
            else if (amount <= 5000)
                return (decimal)5.00;
            else if (amount <= 7500)
                return (decimal)7.50;
            else if (amount <= 10000)
                return (decimal)10.00;
            else
                return (decimal)0.09 / (decimal)100 * amount;
        }

        public void Process(Vault vault, StockPrice stockPrice)
        {
            //No Current Transaction
            if (vault.CurrentTransaction == null)
            {
                var stockBuyCount = (int)Math.Floor(vault.Money / stockPrice.Price);
                var transaction = Create(TransactionType.Buy, stockBuyCount, stockPrice.Date, stockPrice.Price);

                vault.Add(transaction);
            }
            else
            {
                var highThresholdPrice = vault.CurrentTransaction.Price * ((decimal)100 + highSellingThreshold) / (decimal)100;
                var lowThresholdPrice = vault.CurrentTransaction.Price * ((decimal)100 - lowSellingThreshold) / (decimal)100;

                decimal sellingPrice = decimal.MinusOne;

                if (stockPrice.Price > highThresholdPrice)
                    sellingPrice = highThresholdPrice;
                else if (stockPrice.Price < lowThresholdPrice)
                    sellingPrice = lowThresholdPrice;

                if (sellingPrice != decimal.MinusOne)
                {
                    var closingTransaction = Create(TransactionType.Sell, vault.CurrentTransaction.Count,
                        stockPrice.Date, sellingPrice);

                    vault.Add(closingTransaction);
                }
            }
        }

        private static Transaction Create(TransactionType transType, int count, DateTime date, decimal price)
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

        private enum TransactionType
        {
            Buy,
            Sell
        }
    }
}
