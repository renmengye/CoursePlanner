using Panta.Indexing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Panta.Tests
{
    internal class IndexableItem : IIndexable
    {
        public uint ID { get; set; }
        internal string[] Strings { get; set; }
        public IEnumerable<string> GetSplittedIndexStrings()
        {
            return this.Strings;
        }
    }
}
