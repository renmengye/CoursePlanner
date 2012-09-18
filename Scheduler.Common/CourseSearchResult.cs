using Panta.DataModels;
using Panta.Indexing;
using Panta.Searching;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Scheduler.Common
{
    [DataContract]
    public class CourseSearchResult : SearchResult
    {
        [DataMember]
        public string CodeNameMatches { get; set; }

        [DataMember]
        public string DescriptionMatches { get; set; }

        [DataMember]
        public string PrerequisiteMatches { get; set; }

        [DataMember]
        public string PostrequisiteMatches { get; set; }

        [DataMember]
        public string DepartmentMatches { get; set; }

        [DataMember]
        public string RawMatches { get; set; }

        public override string Matches
        {
            get
            {
                return null;
            }
            set
            {
                this.RawMatches = value;
            }
        }
    }
}
