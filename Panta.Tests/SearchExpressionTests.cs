using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Panta.Indexing;
using Panta.Indexing.Correctors;
using Panta.Indexing.Expressions;

namespace Panta.Tests
{
    [TestClass]
    public class SearchExpressionTests
    {
        [TestMethod]
        public void SearchExpressionBasics()
        {
            string query;
            query = "hi good no";
            Assert.AreEqual("((hi && good) && no)", SearchExpression.Parse(query, null).ToString());
            query = "hi|good no";
            Assert.AreEqual("((hi || good) && no)", SearchExpression.Parse(query, null).ToString());
            query = "-hi|good no";
            Assert.AreEqual("(( || good) && no)", SearchExpression.Parse(query, null).ToString());
            query = "hi|good -no";
            Assert.AreEqual("((hi || good) && !no)", SearchExpression.Parse(query, null).ToString());
        }

        [TestMethod]
        public void SearchExpressionNull()
        {
            string query = null;
            Assert.AreEqual("", SearchExpression.Parse(query, null).ToString());
        }
    }
}
