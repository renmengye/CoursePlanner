using System.Collections.Generic;
using System;

namespace Panta.Indexing
{
    public interface IIndexable
    {
        uint ID { get; set; }
        IEnumerable<string> GetSplittedIndexStrings();
    }
}
