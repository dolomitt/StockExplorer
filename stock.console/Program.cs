using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using stock.Domain;
using stock.Engine;
using stock.Strategy;

namespace stock.console
{
    public class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                var result = new List<string>();

                const string transactionSource = @"C:\Users\tgravran\Documents\visual studio 2015\Projects\SolutionStockExplorer\stock.console\data_total.csv";
                const string resultFile = @"C:\Users\tgravran\Documents\visual studio 2015\Projects\SolutionStockExplorer\stock.console\resultFile.csv";

                IDataLoader loader = new DataLoader.DataLoader(transactionSource);
                ITransactionFactory factory = new TransactionFactory();

                decimal amountIncrement = (decimal)1000;
                decimal amountMin = (decimal)1000;
                decimal amountMax = (decimal)30000;

                decimal highPriceMin = (decimal)0;
                decimal highPriceMax = (decimal)2;
                decimal highPriceIncrement = (decimal)0.1;

                decimal lowPriceMin = (decimal)0;
                decimal lowPriceMax = (decimal)2;
                decimal lowPriceIncrement = (decimal)0.1;

                double totalCount = Convert.ToDouble((amountMax - amountMin) / amountIncrement * ((highPriceMax - highPriceMin) / highPriceIncrement) * ((lowPriceMax - lowPriceMin) / lowPriceIncrement));

                var tasks = new List<Task>();

                //for (var initialAmount = amountMin; initialAmount < amountMax; initialAmount += amountIncrement)
                //    for (var highPrice = highPriceMin; highPrice < highPriceMax; highPrice += highPriceIncrement)
                //        for (var lowPrice = lowPriceMin; lowPrice < lowPriceMax; lowPrice += lowPriceIncrement)
                //        {
                //            var c = new ProcessCondition()
                //            {
                //                HighPrice = highPrice,
                //                LowPrice = lowPrice,
                //                InitialAmount = initialAmount
                //            };

                //            var t = Task.Run(() =>
                //            {
                //                var processor = new TransactionProcessor(c);

                //                var enumerator = loader.Read();

                //                while (enumerator.MoveNext())
                //                    processor.Process(processor.Vault, enumerator.Current);

                //                result.Add(string.Format("{0},{1},{2},{3},{4},{5}", c.InitialAmount, c.HighPrice, c.LowPrice, processor.Vault.GetMargin(), processor.Vault.GetTotalBankFees(), processor.Vault.GetTransactionCount()));
                //                Console.WriteLine("{0},{1},{2},{3},{4},{5}", c.InitialAmount, c.HighPrice, c.LowPrice, processor.Vault.GetMargin(), processor.Vault.GetTotalBankFees(), processor.Vault.GetTransactionCount());
                //            });

                //            tasks.Add(t);
                //        }

                var c = new ProcessCondition()
                {
                    HighPrice = (decimal)1.8,
                    LowPrice = (decimal)0.2,
                    InitialAmount = (decimal)3000
                };

                var t = Task.Run(() =>
                {
                    IStockTradingStrategy strategy = new StandardStockStrategy(ComputeTransactionFee);

                    var processor = new TransactionProcessor(ComputeTransactionFee, strategy, factory, c);
                    var enumerator = loader.Read();

                    while (enumerator.MoveNext())
                        processor.Process(enumerator.Current);

                    result.Add(string.Format("{0},{1},{2},{3},{4},{5},{6}", c.InitialAmount, c.HighPrice, c.LowPrice, processor.Vault.GetMargin(), processor.Vault.GetTotalBankFees(), processor.Vault.GetTransactionCount(), processor.Vault.Money));
                    Console.WriteLine("{0},{1},{2},{3},{4},{5},{6}", c.InitialAmount, c.HighPrice, c.LowPrice, processor.Vault.GetMargin(), processor.Vault.GetTotalBankFees(), processor.Vault.GetTransactionCount(), processor.Vault.Money);
                });

                tasks.Add(t);

                Task.WaitAll(tasks.ToArray());

                if (File.Exists(resultFile))
                    File.Delete(resultFile);

                //Insert Header
                result.Insert(0, string.Format("{0},{1},{2},{3},{4},{5},{6}", "InitialAmount", "High", "Low", "Margin", "BankFees", "TransactionCount", "Money"));

                //Write to file
                File.AppendAllLines(resultFile, result);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            Console.WriteLine("Process Over");
            Console.ReadKey();
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
    }
}

