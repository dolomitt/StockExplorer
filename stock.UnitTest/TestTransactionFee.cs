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
            var bankFee = TransactionProcessor.ComputeTransactionFee(1000);
            Assert.AreEqual(bankFee, (decimal)2.5);
        }

        [TestMethod]
        public void TransactionFeeAmountGreaterThan10000()
        {
            var bankFee = TransactionProcessor.ComputeTransactionFee(12000);
            Assert.AreEqual(bankFee, (decimal)12000* (decimal)0.09 / (decimal)100);
        }
    }
}
