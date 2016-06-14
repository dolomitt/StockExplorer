using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using stock.Engine;

namespace stock.UnitTest
{
    [TestClass]
    public class TestTransactionFee
    {
        [TestMethod]
        public void TransactionFeeAmountEqualsTo1000()
        {
            var bankFee = ComputeTransactionFee(1000);
            Assert.AreEqual(bankFee, (decimal)2.5);
        }

        [TestMethod]
        public void TransactionFeeAmountGreaterThan10000()
        {
            var bankFee = ComputeTransactionFee(12000);
            Assert.AreEqual(bankFee, (decimal)12000* (decimal)0.09 / (decimal)100);
        }

        private static decimal ComputeTransactionFee(decimal amount)
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
