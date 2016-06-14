using System;

namespace stock.Domain
{
    public interface IStockTradingStrategy
    {
        Transaction Process(ITransactionFactory factory, Vault vault, StockPrice stockPrice);
    }
}