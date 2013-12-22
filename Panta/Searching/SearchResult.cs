using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Panta.Searching
{
    [DataContract]
    [Serializable]
    [KnownType(typeof(CourseSearchResult))]
    public class SearchResult
    {
        [DataMember]
        public virtual string Matches { get; set; }
    }

    [DataContract]
    [Serializable]
    public class SimpleCourse
    {
        [DataMember]
        [JsonProperty("cid")]
        public uint Id { get; set; }

        [DataMember]
        [JsonProperty("abbr")]
        public string Abbr { get; set; }

        [DataMember]
        [JsonProperty("name")]
        public string Name { get; set; }
    }

    [DataContract]
    [Serializable]
    public class RawCourseSearchResult
    {
        [DataMember]
        public virtual SimpleCourse Matches { get; set; }
    }
}
