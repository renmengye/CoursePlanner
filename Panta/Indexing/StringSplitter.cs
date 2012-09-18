using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Panta.Indexing
{
    /// <summary>
    /// Split strings based on alphanumeric rules.
    /// Filter out the common words.
    /// </summary>
    public class StringSplitter
    {
        private static string[] ExcludedWords = { "a", "the", "to", "in", "by", "is", "and", "this", "gt", "of", "lt", "for", "not", "it", "on", "with", "new", "be", "as", "that", "from", "we", "you", "am", "when", "if", "are", "no", "see", "at", "or", "up", "will", "an", "have", "but", "after", "was", "text", "can", "like", "there", "then", "has", "http", "so", "do", "one", "same", "which", "steps", "now", "also", "into", "still", "back", "need", "end", "use", "set", "due", "our", "out", "could", "should", "expect", "more", "any" };
        private static HashSet<string> _excluded;

        static StringSplitter()
        {
            _excluded = new HashSet<string>();
            foreach (string word in ExcludedWords)
                _excluded.Add(word);
        }

        public static ICollection<string> Split(string s)
        {
            List<string> results = new List<string>();
            int alphaStart = 0;

            // Knows if is in a alphanumeric group
            bool inAlpha = false;

            for (int i = 0; i < s.Length; i++)
            {
                if (ShouldIndex(s[i]))
                {
                    if (!inAlpha)
                    {
                        inAlpha = true;
                        alphaStart = i;
                    }
                }
                else
                {
                    if (inAlpha)
                    {
                        Add(s, alphaStart, i - 1, results);
                        inAlpha = false;
                    }
                }
            }
            // Check if there are still string remaining
            if (inAlpha)
            {
                Add(s, alphaStart, s.Length - 1, results);
            }
            return results;
        }

        private static bool ShouldIndex(char c)
        {
            return Char.IsLetterOrDigit(c);
        }

        private static void Add(string s, int start, int end, ICollection<string> set)
        {
            string toAdd = s.Substring(start, end - start + 1).ToLowerInvariant();
            if (!_excluded.Contains(toAdd))
            {
                set.Add(toAdd);
            }
        }

        public static void SeparatePrefix(string value, out string prefix, out string root)
        {
            // Default: No prefix, root is full value
            prefix = String.Empty;
            root = value;

            // If a ':' is found, separate the prefix and ':' from the rest
            if (!String.IsNullOrEmpty(root) && root.Contains(":") && !root.EndsWith(":"))
            {
                prefix = root.Substring(0, root.IndexOf(':') + 1);
                root = root.Substring(prefix.Length);
            }
        }
    }
}
