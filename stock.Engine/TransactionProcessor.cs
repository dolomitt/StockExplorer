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
        public Vault Vault => vault;
        private readonly Vault vault;

        private readonly IStockTradingStrategy strategy;
        private readonly ITransactionFactory factory;

        public TransactionProcessor(Func<decimal, decimal> computeTransactionFee, IStockTradingStrategy strategy, ITransactionFactory factory, ProcessCondition c)
        {
            this.strategy = strategy;
            this.factory = factory;
            vault = new Vault(c.InitialAmount, computeTransactionFee);
        }

        public void Process(StockPrice stockPrice)
        {
            try
            {
                var transaction = strategy.Process(this.factory, vault, stockPrice);
                var result = vault.Add(transaction);

                if (!result)
                    throw new TransactionRefusedException() { Transaction = transaction };

            }
            catch (TransactionRefusedException ex)
            {
                Console.WriteLine(ex.ToString());
                throw;
            }
        }
    }
}
