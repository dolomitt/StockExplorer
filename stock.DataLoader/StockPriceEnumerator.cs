using System;
using System.Collections;
using System.Collections.Generic;
using stock.Domain;

namespace stock.DataLoader
{
    public class StockPriceEnumerator : IEnumerator<StockPrice>
    {
        private int currentPosition;
        private int currentIndex;

        private readonly List<StockPriceData> data;

        public StockPriceEnumerator(List<StockPriceData> data)
        {
            this.data = data;
            currentPosition = -1;
            currentIndex = -1;
        }

        public StockPrice Current
        {
            get
            {
                return GetPrice(currentPosition, currentIndex);
            }
        }

        object IEnumerator.Current => Current;

        public void Reset()
        {
            currentPosition = -1;
            currentIndex = -1;
        }

        void IDisposable.Dispose() { }

        public bool MoveNext()
        {
            if (currentIndex == 3)
            {
                currentPosition++;
                currentIndex = 0;

                if (data != null && data.Count > currentPosition)
                    return true;
                else
                    return false;
            }
            else
            {
                if (currentPosition == -1)
                    currentPosition = 0;

                currentIndex++;
                return true;
            }
        }

        private StockPrice GetPrice(int position, int index)
        {
            if (index == 0)
                return new StockPrice() {Date = data[position].Date.AddHours(9), Price = data[position].Open};
            else if (index == 1)
                return new StockPrice() { Date = data[position].Date.AddHours(11), Price = data[position].Low };
            else if (index == 2)
                return new StockPrice() { Date = data[position].Date.AddHours(15), Price = data[position].High };
            else if (index == 3)
                return new StockPrice() {Date = data[position].Date.AddHours(17), Price = data[position].Close};
            else
                return null;
        }
    }
}