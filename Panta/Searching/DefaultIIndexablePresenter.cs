using Panta.DataModels;
using Panta.Indexing;
using Panta.Indexing.Correctors;
using Panta.Indexing.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Panta.Searching
{
    public class DefaultIIndexablePresenter<T> : IIndexablePresenter<T> where T : IIndexable
    {
        public InvertedWordIndex Index { get; set; }
        public IIndexableCollection<T> ItemCollection { get; set; }
        public ITermCorrector Corrector { get; set; }

        public DefaultIIndexablePresenter(InvertedWordIndex index, IIndexableCollection<T> collection)
        {
            this.Index = index;
            this.ItemCollection = collection;
            this.Corrector = new SuffixCorrector(this.Index.SortedKeys);
        }

        public HashSet<uint> GetIDMatches(string query, string prefix)
        {
            return GetIDMatches(query, prefix, this.Corrector);
        }

        public HashSet<uint> GetIDMatches(string query, string prefix, ITermCorrector corrector)
        {
            if (prefix == null)
            {
                return SearchExpression.Parse(query, corrector).Evaluate(this.Index);
            }
            else
            {
                return SearchExpression.ParseEachTermWithPrefix(query, prefix, corrector).Evaluate(this.Index);
            }
        }

        public virtual IEnumerable<T> GetItemsFromIDs(IEnumerable<uint> ids)
        {
            List<T> result = new List<T>();
            foreach (uint id in ids)
            {
                T item;
                if (this.ItemCollection.TryGetItem(id, out item)) result.Add(item);
            }
            return result;
        }

        public virtual string FormatList(IEnumerable<T> items)
        {
            throw new NotImplementedException();
        }

        public virtual SearchResult GetItemList(string query)
        {
            throw new NotImplementedException();
        }
    }
}
