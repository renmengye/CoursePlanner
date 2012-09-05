using Panta.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Panta.Fetchers
{
    public interface IItemFetcher<T>
    {
        string Url { get; }
        IEnumerable<T> FetchItems();
    }
}
