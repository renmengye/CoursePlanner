using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Panta.Indexing;

namespace Panta.Tests
{
    [TestClass]
    public class IndexStringTests
    {
        [TestMethod]
        public void ToSplittedStringsBasic()
        {
            // One item
            IndexString test = new IndexString("good:", "bad");
            Assert.AreEqual("bad good:bad", String.Join(" ", test.ToSplittedStrings()));
        }

        [TestMethod]
        public void ToSplittedStringsTwoRoots()
        {
            // More than one item
            IndexString test = new IndexString("good:", "bad?ok");
            Assert.AreEqual("bad good:bad ok good:ok", String.Join(" ", test.ToSplittedStrings()));
        }

        [TestMethod]
        public void ToSplittedStringsNoPrefix()
        {
            // No prefix
            IndexString test = new IndexString(null, "good-ok");
            Assert.AreEqual("good-ok", String.Join(" ", test.ToSplittedStrings()));
        }

        [TestMethod]
        public void ToSplittedStringsNoRoot()
        {
            // No root
            IndexString test = new IndexString("good", null);
            Assert.AreEqual("", String.Join(" ", test.ToSplittedStrings()));
        }
    }
}
