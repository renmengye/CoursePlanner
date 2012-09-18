using Panta.Indexing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Panta.DataModels
{
    [DataContract]
    [Serializable]
    public class SchoolProgram : IIndexable
    {
        [DataMember]
        public uint ID{get;set;}

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string Description { get; set; }

        protected virtual IList<IndexString> GetIndexStrings()
        {
            IList<IndexString> strings = new List<IndexString>();
            strings.Add(new IndexString("id:", this.ID.ToString()));
            strings.Add(new IndexString("name:", this.Name));
            strings.Add(new IndexString("des:", this.Description));

            return strings;
        }

        public IEnumerable<string> GetSplittedIndexStrings()
        {
            List<string> results = new List<string>();
            foreach (IndexString istring in this.GetIndexStrings())
            {
                results.AddRange(istring.ToSplittedStrings());
            }

            return results;
        }
    }
}
