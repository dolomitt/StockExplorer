using System;

namespace stock.Domain
{
    public interface ITransactionFactory
    {
        Transaction Create(TransactionType transType, long count, DateTime date, decimal price);
    }
}