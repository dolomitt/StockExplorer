using System;
using stock.Domain;

namespace stock.Engine
{
    public class TransactionRefusedException : Exception
    {
        public Transaction Transaction { get; set; }
    }
}