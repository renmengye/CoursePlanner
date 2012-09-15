using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Panta.Indexing
{
    public interface IIndexableCollection<T> where T : IIndexable
    {
        Dictionary<uint, T> IndexableItemsCatalog { get; set; }
        bool TryGetItem(uint id, out T item);
    }
}
