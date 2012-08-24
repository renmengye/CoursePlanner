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

            if (root.Length >= 2)
            {
                IExpression results = new TermExpression(root);
                foreach (string correction in FindStartsWith(term))
                {
                    if (!correction.Equals(root)) results = LogicOrExpression.Join(results, new TermExpression(correction));
                }
                return results;
            }

            return new TermExpression(String.Empty);
        }

        public IEnumerable<string> FindStartsWith(string term)
        {
            int startIndex = Array.BinarySearch<string>(SortedWords, term, StringComparer.Ordinal);
            if (startIndex < 0) startIndex = ~startIndex;

            for (int i = startIndex; i < SortedWords.Length; i++)
            {
                if (SortedWords[i].StartsWith(term))
                {
                    yield return SortedWords[i];
                }
                else
                {
                    break;
                }
            }
        }
    }
}
