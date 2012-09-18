using Panta.Indexing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Panta.DataModels
{
    public interface IIndexableCollection<T> where T : IIndexable
    {
        Dictionary<uint, T> IIndexableItemsCatalog { get; set; }
        bool TryGetItem(uint id, out T item);
    }
}
