using Microsoft.VisualStudio.TestTools.UnitTesting;
using Panta.Indexing;
using Panta.Indexing.Correctors;
using Panta.Indexing.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Panta.Tests
{
    [TestClass]
    public class SuffixCorrectorTests
    {
        static InvertedWordIndex index;
        static SuffixCorrector corrector;
        static IndexableItem[] indexableItems;

        [ClassInitialize]
        public static void Init(TestContext context)
        {
            indexableItems = new IndexableItem[5];
            for (int i = 0; i < 5; i++)
            {
                indexableItems[i] = new IndexableItem()
                {
                    ID = (uint)i
                };
            }
            index = new InvertedWordIndex();
            indexableItems[0].Strings = new string[] { "000", "001" };
            indexableItems[1].Strings = new string[] { "011", "110" };
            indexableItems[2].Strings = new string[] { "12111", "12555" };
            indexableItems[3].Strings = new string[] { "01156", "12666" };
            indexableItems[4].Strings = new string[] { "12e8", "0" };
            foreach (IndexableItem item in indexableItems)
            {
                index.Add(item);
            }

            corrector = new SuffixCorrector(index.SortedKeys);
        }

        [TestMethod]
        public void SuffixCorrectorBasic()
        {
            Assert.AreEqual("011 01156", String.Join(" ", corrector.Correct("01")));
        }

        [TestMethod]
        public void SuffixCorrectorSizeBound()
        {
            // Hit the upper bound of the sorted keys
            Assert.AreEqual("12111 12555 12666 12e8", String.Join(" ", corrector.Correct("12")));

            // Hit the lower bound
            Assert.AreEqual("000 001", String.Join(" ", corrector.Correct("00")));
        }

        [TestMethod]
        public void SuffixCorrectorShort()
        {
            // Does not correct less than two characters
            Assert.AreEqual("", String.Join(" ", corrector.Correct("0")));
        }
    }
}
