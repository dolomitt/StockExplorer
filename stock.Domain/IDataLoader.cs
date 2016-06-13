using System.Collections.Generic;

namespace stock.Domain
{
    public interface IDataLoader
    {
        IEnumerator<StockPrice> Read();
    }
}