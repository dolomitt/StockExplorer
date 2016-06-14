using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Policy;
using stock.Domain;

namespace stock.DataLoader
{
    public class DataLoader : IDataLoader
    {
        private readonly string filename;
        private List<StockPriceData> dataSource;

        public DataLoader(string filename)
        {
            this.filename = filename;
            this.LoadData();
        }

        public IEnumerator<StockPrice> Read()
        {
            return new StockPriceEnumerator(this.dataSource);
        }

        private void LoadData()
        {
            Console.WriteLine("DataLoader : Loading data..");

            var result = new List<StockPriceData>();
            var dataTotal = new FileInfo(this.filename);
            var cultureInfo = new System.Globalization.CultureInfo("fr-FR");
            var lineNumber = 0;

            using (var reader = dataTotal.OpenText())
            {
                while (!reader.EndOfStream)
                {
                    lineNumber++;

                    var line = reader.ReadLine();

                    if (line == null)
                        continue;

                    var data = line.Split('\t');

                    if (lineNumber == 1)
                        continue;

                    var linedata = new StockPriceData
                    {
                        Date = DateTime.Parse(data[0], cultureInfo),
                        Open = decimal.Parse(data[1]),
                        High = decimal.Parse(data[2]),
                        Low = decimal.Parse(data[3]),
                        Close = decimal.Parse(data[4]),
                        Volume = long.Parse(data[5]),
                        AdjClose = decimal.Parse(data[6])
                    };

                    if (linedata.Date > new DateTime(2016,1,1))
                        result.Add(linedata);
                }
            }

            Console.WriteLine("DataLoader : Data Loaded");
            dataSource = result.OrderBy(i => i.Date).ToList();
        }
    }
}