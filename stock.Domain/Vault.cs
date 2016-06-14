using System;
using System.Collections.Generic;
using System.Linq;

namespace stock.Domain
{
    public class Vault
    {
        public decimal Money => money;
        public decimal InitialMoney => initialMoney;
        public Transaction CurrentTransaction => currentTransaction;

        private decimal money { get; set; }
        private decimal initialMoney { get; set; }
        private Transaction currentTransaction { get; set; }

        readonly Func<decimal, decimal> computeBankFees;

        private List<Transaction> transactions
        {
            get;
            set;
        }

        public Vault(decimal initialAmount, Func<decimal, decimal> computeBankFees)
        {
            if (computeBankFees == null)
                throw new ArgumentNullException("computeBankFees");

            this.initialMoney = initialAmount;
            this.computeBankFees = computeBankFees;
            this.money = initialAmount;

            this.transactions = new List<Transaction>();
        }

        public decimal GetMargin()
        {
            return this.Money - this.InitialMoney;
        }

        public void DisplaySummary()
        {
            Console.WriteLine("Summary - Current Position:{0:C2} - Margin:{1:C2}", this.Money, GetMargin());
            Console.WriteLine();
        }

        public decimal GetTotalBankFees()
        {
            return transactions.Sum(t => t.BankFee);
        }

        public int GetTransactionCount()
        {
            return transactions.Count();
        }

        public bool Add(Transaction transaction)
        {
            transaction.BankFee = computeBankFees(transaction.GetAmount());

            if (transaction is BuyTransaction)
            {
                //Have enough Money ?
                if (this.currentTransaction == null && (transaction.GetAmount() + transaction.BankFee) <= money)
                {
                    this.transactions.Add(transaction);

                    money -= transaction.GetAmount() + transaction.BankFee;
                    currentTransaction = transaction;

                    return true;
                }
                else
                {
                    return false;
                }
            }
            else if (transaction is SellTransaction)
            {
                //Have enough titles ?
                if (this.currentTransaction != null && transaction.Count <= this.currentTransaction.Count)
                {
                    this.transactions.Add(transaction);

                    money += transaction.GetAmount() - transaction.BankFee;
                    transaction.LinkedTransaction = currentTransaction;
                    currentTransaction = null;

                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
    }
}
