using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Panta.Searching
{
    [DataContract]
    public class SearchResult
    {
        [DataMember]
        public virtual string Matches { get; set; }
    }
}
