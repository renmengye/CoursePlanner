using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Panta.Indexing
{
    /// <summary>
    /// A structure that can hold a prefix string and a root string.
    /// Prefixes mainly for defining the type of root data, for more accurate search results
    /// </summary>
    public struct IndexString
    {
        public string Prefix;
        public string Root;

        public IndexString(string prefix, string root)
        {
            this.Root = String.Empty;
            this.Prefix = String.Empty;
            if (prefix != null)
            {
                Prefix = prefix;
            }
            if (root != null)
            {
                Root = root;
            }
        }

        public override string ToString()
        {
            return Prefix + Root;
        }

        /// <summary>
        /// Parse the root string and form the concatenation of prefix and the parsed parts of the root for inverted word index
        /// </summary>
        /// <returns>A list of part of the root with the same prefix</returns>
        public IEnumerable<string> ToSplittedStrings()
        {
            List<string> results = new List<string>();
            foreach(string part in StringSplitter.Split(this.Root)){
                results.Add(part);
                if (!String.IsNullOrEmpty(this.Prefix))
                {
                    results.Add(this.Prefix + part);
                }
            }
            return results;
        }
    }
}
