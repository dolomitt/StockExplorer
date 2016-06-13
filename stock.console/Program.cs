using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using stock.Domain;
using stock.Engine;

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

                for (var initialAmount = (decimal)1000.0; initialAmount < (decimal)30000; initialAmount += 1000)
                { 
                    for (var highPrice = (decimal)0.00; highPrice < (decimal)2; highPrice += (decimal)0.05)
                    {
                        for (var lowPrice = (decimal) 0.00; lowPrice < (decimal) 2; lowPrice += (decimal)0.05)
                        {
                            var processor = new TransactionProcessor(initialAmount, highPrice, lowPrice);

                            var enumerator = loader.Read();

                            while (enumerator.MoveNext())
                                processor.Process(processor.Vault, enumerator.Current);

                            result.Add(string.Format("{0},{1},{2},{3},{4},{5}", initialAmount, highPrice, lowPrice, processor.Vault.GetMargin(), processor.Vault.GetTotalBankFees(), processor.Vault.GetTransactionCount()));
                        }
                    }
                }

                if (File.Exists(resultFile))
                    File.Delete(resultFile);

                //Insert Header
                result.Insert(0, string.Format("{0},{1},{2},{3},{4},{5}", "InitialAmount", "High", "Low", "Margin", "BankFees", "TransactionCount"));

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
    }
}

