using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Panta.Indexing;

namespace Panta.Tests
{
    [TestClass]
    public class InvertedWordIndexTests
    {
        InvertedWordIndex index;

        [TestInitialize]
        public void Init()
        {
            index = new InvertedWordIndex();

            IndexableItem item1 = new IndexableItem();
            IndexableItem item2 = new IndexableItem();

            item1.ID = 0;
            item2.ID = 1;

            item1.Strings = new string[] { "good0", "bad0", "bad1" };
            item2.Strings = new string[] { "good1", "bad1" };

            index.Add(item1);
            index.Add(item2);
        }

        [TestMethod]
        public void GetMatchedIDsBasic()
        {
            Assert.AreEqual("0", String.Join(" ", index.GetMatchedIDs("good0")));
        }

        [TestMethod]
        public void GetMatchedIDsEmpty()
        {
            Assert.AreEqual("", String.Join(" ", index.GetMatchedIDs("good2")));
        }

        [TestMethod]
        public void GetMatchedIDsMultiple()
        {
            Assert.AreEqual("0 1", String.Join(" ", index.GetMatchedIDs("bad1")));
        }

        [TestMethod]
        public void AddEmpty()
        {
            IndexableItem item3 = new IndexableItem();
            item3.ID = 2;
            item3.Strings = new string[] { "good2" };
            index.Add(item3);
            Assert.AreEqual("bad0 bad1 good0 good1 good2", String.Join(" ", index.SortedKeys));
            Assert.AreEqual(1, index.GetMatchedIDs("good2").Count);
        }

        [TestMethod]
        public void AddExisted()
        {
            IndexableItem item3 = new IndexableItem();
            item3.ID = 2;
            item3.Strings = new string[] { "good1" };
            index.Add(item3);
            Assert.AreEqual("bad0 bad1 good0 good1", String.Join(" ", index.SortedKeys));
            Assert.AreEqual("1 2", String.Join(" ", index.GetMatchedIDs("good1")));
        }
    }
}
