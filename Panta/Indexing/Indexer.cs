using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Panta.Indexing
{
    public class Indexer<T> where T : IIndexable
    {
        private InvertedWordIndex InvertedIndex { get; set; }

        public Indexer()
        {
            this.InvertedIndex = new InvertedWordIndex();
        }

        public Indexer(string name)
        {
            this.InvertedIndex = new InvertedWordIndex(name);
        }

        public void Index(IEnumerable<T> items)
        {
            foreach (T item in items)
            {
                InvertedIndex.Add(item);
            }
            this.InvertedIndex.Save();
        }
    }
}
