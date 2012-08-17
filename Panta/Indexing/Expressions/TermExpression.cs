using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Panta.Indexing.Expressions
{
    public class TermExpression : IExpression
    {
        private string Term { get; set; }

        public IEnumerable<string> Terms
        {
            get
            {
                return new string[] { Term };
            }
        }

        public TermExpression(string term)
        {
            Term = term;
        }

        public HashSet<uint> Evaluate(IIdProvider provider)
        {
            HashSet<uint> results = new HashSet<uint>();
            ICollection<uint> tryResults;
            if (provider.TryGetValue(this.Term, out tryResults))
            {
                results.UnionWith(tryResults);
            }
            return results;
        }
    }
}
