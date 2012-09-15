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
    public class SearchEngine<T> where T : IIndexable
    {
        private InvertedWordIndex Index { get; set; }
        private IIndexableCollection<T> Collection { get; set; }
        private ITermCorrector Corrector { get; set; }

        public SearchEngine(InvertedWordIndex index, IIndexableCollection<T> collection)
        {
            this.Index = index;
            this.Collection = collection;
            this.Corrector = new SuffixCorrector(this.Index.SortedKeys);
        }

        public IEnumerable<T> Search(string query)
        {
            //IExpression rawExpression = SearchExpression.Parse(query, this.Corrector);
            //HashSet<uint> rawMatches = rawExpression.Evaluate(this.Index);

            //IExpression codeExpression = SearchExpression.ParseEachTermWithPrefix(query, "code:", this.Corrector);
            //HashSet<uint> codeMatches = codeExpression.Evaluate(this.Index);

            //IExpression nameExpression = SearchExpression.ParseEachTermWithPrefix(query, "name:", this.Corrector);
            //HashSet<uint> nameMatches = nameExpression.Evaluate(this.Index);

            //rawMatches.ExceptWith(codeMatches);
            //rawMatches.ExceptWith(nameMatches);
            //nameMatches.ExceptWith(codeMatches);

            //SearchResult<T> result = new SearchResult<T>();
            //foreach (uint codeMatch in codeMatches)
            //{
            //    T item;
            //    if (this.Collection.TryGetItem(codeMatch, out item)) result.PrefixMatches.Add(item);
            //}
            //foreach (uint nameMatch in nameMatches)
            //{
            //    T item;
            //    if (this.Collection.TryGetItem(nameMatch, out item)) result.PrefixMatches.Add(item);
            //}
            //foreach (uint rawMatch in rawMatches)
            //{
            //    T item;
            //    if (this.Collection.TryGetItem(rawMatch, out item)) result.RawMatches.Add(item);
            //}
            IExpression expresion;
            expresion = SearchExpression.Parse(query, this.Corrector);

            List<T> result = new List<T>();
            foreach (uint match in expresion.Evaluate(this.Index))
            {
                T item;
                if (this.Collection.TryGetItem(match, out item)) result.Add(item);
            }

            return result;
        }
    }
}
