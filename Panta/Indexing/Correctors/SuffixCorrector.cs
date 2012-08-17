using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Panta.Indexing.Expressions;

namespace Panta.Indexing.Correctors
{
    public class SuffixCorrector : ITermCorrector
    {
        private string[] SortedWords { get; set; }

        public SuffixCorrector(string[] sortedWords)
        {
            SortedWords = sortedWords;
        }

        public IExpression Correct(string term)
        {
            string prefix, root;
            StringSplitter.SeparatePrefix(term, out prefix, out root);

            if (root.Length > 2)
            {
                IExpression results = null;
                foreach (string correction in FindStartsWith(term))
                {
                    results = LogicOrExpression.Join(results, new TermExpression(correction));
                }
                return results;
            }

            return null;
        }

        public IEnumerable<string> FindStartsWith(string term)
        {
            int startIndex = Array.BinarySearch<string>(SortedWords, term, StringComparer.Ordinal);
            if (startIndex < 0) startIndex = ~startIndex;

            for (int i = startIndex; SortedWords[i].StartsWith(term); i++)
            {
                yield return SortedWords[i];
            }
        }
    }
}
