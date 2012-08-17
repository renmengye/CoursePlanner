using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Panta.Indexing
{
    /// <summary>
    /// Can provide a list of IDs based on a key
    /// </summary>
    public interface IIdProvider
    {
        bool TryGetValue(string key, out ICollection<uint> value);
    }
}
