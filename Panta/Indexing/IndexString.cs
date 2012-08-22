using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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

        public IEnumerable<string> ToSplittedStrings()
        {
            List<string> results = new List<string>();
            foreach(string part in StringSplitter.Split(this.Root)){
                results.Add(part);
                results.Add(this.Prefix + part);
            }
            return results;
        }
    }
}
