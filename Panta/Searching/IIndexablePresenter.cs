using Panta.DataModels;
using Panta.Indexing;
using Panta.Indexing.Correctors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Panta.Searching
{
    public interface IIndexablePresenter<T> where T : IIndexable
    {
        InvertedWordIndex Index { get; set; }
        IIndexableCollection<T> ItemCollection { get; set; }
        ITermCorrector Corrector { get; set; }
        
        HashSet<uint> GetIDMatches(string query, string prefix, ITermCorrector corrector);
        IEnumerable<T> GetItemsFromIDs(IEnumerable<uint> ids);
        string FormatList(IEnumerable<T> items);
        SearchResult GetItemList(string query);
    }
}
