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

        public TransactionProcessor(ProcessCondition c)
        {
            this.highSellingThreshold = c.HighPrice;
            this.lowSellingThreshold = c.LowPrice;
            vault = new Vault(c.InitialAmount, ComputeTransactionFee);
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
            try
            {
                //No Current Transaction
                if (vault.CurrentTransaction == null)
                {
                    var stockBuyCount = (long)Math.Floor(vault.Money / stockPrice.Price);
                    var transaction = Create(TransactionType.Buy, stockBuyCount, stockPrice.Date, stockPrice.Price);

                    while (transaction.GetAmount() + ComputeTransactionFee(transaction.GetAmount()) > vault.Money)
                    { 
                        stockBuyCount--;
                        transaction = Create(TransactionType.Buy, stockBuyCount, stockPrice.Date, stockPrice.Price);
                    }

                    bool result = vault.Add(transaction);

                    if (!result)
                        throw new TransactionRefusedException() {Transaction = transaction};
                }
                else
                {
                    var highThresholdPrice = vault.CurrentTransaction.Price * ((decimal)100 + highSellingThreshold) / (decimal)100;
                    var lowThresholdPrice = vault.CurrentTransaction.Price * ((decimal)100 - lowSellingThreshold) / (decimal)100;

                    var sellingPrice = decimal.MinusOne;

                    if (stockPrice.Price > highThresholdPrice)
                        sellingPrice = highThresholdPrice;
                    else if (stockPrice.Price < lowThresholdPrice)
                        sellingPrice = lowThresholdPrice;

                    if (sellingPrice != decimal.MinusOne)
                    {
                        var transaction = Create(TransactionType.Sell, vault.CurrentTransaction.Count,
                            stockPrice.Date, sellingPrice);

                        var result = vault.Add(transaction);

                        if (!result)
                            throw new TransactionRefusedException() { Transaction = transaction };
                    }
                }
            }
            catch (TransactionRefusedException ex)
            {
                Console.WriteLine(ex.ToString());
                throw;
            }
        }

        private static Transaction Create(TransactionType transType, long count, DateTime date, decimal price)
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

    public class TransactionRefusedException : Exception
    {
        public Transaction Transaction { get; set; }
    }

    public struct ProcessCondition
    {
        public decimal InitialAmount;
        public decimal HighPrice;
        public decimal LowPrice;
    }
}
