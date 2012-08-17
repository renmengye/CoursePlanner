using System.Collections.Generic;
using System;

namespace Panta.Indexing
{
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
    }

    public interface IIndexable
    {
        uint ID { get; set; }
        IEnumerable<IndexString> GetIndexStrings();
    }
}
