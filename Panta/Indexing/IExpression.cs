using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Panta.Indexing
{
    public interface IExpression
    {
        /// <summary>
        /// Filter the id set to the ones that matches this expression
        /// </summary>
        /// <param name="provider">A provider that can provide a set of ids based on search strings</param>
        /// <returns>Filtered set of strings</returns>
        HashSet<uint> Evaluate(IIdProvider provider);

        /// <summary>
        /// Get a collection of terms involved in the expression for building match regex on displaying results
        /// </summary>
        IEnumerable<string> Terms { get; }
    }
}
