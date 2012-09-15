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

        public IEnumerable<string> Correct(string term)
        {
            string prefix, root;
            StringSplitter.SeparatePrefix(term, out prefix, out root);
            List<string> results = new List<string>();

            if (root.Length >= 2)
            {
                foreach (string correction in FindStartsWith(term))
                {
                    if (!correction.Equals(root)) results.Add(correction);
                }
            }
            return results;
        }

        public IEnumerable<string> FindStartsWith(string term)
        {
            int startIndex = Array.BinarySearch<string>(SortedWords, term, StringComparer.Ordinal);
            if (startIndex < 0) startIndex = ~startIndex;

            for (int i = startIndex; i < SortedWords.Length; i++)
            {
                if (SortedWords[i].Replace("-","").StartsWith(term.Replace("-","")))
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
