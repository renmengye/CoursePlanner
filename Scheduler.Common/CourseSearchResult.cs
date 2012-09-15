using Panta.DataModels;
using Panta.Indexing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Scheduler.Common
{
    public class CourseSearchResult
    {
        public string CodeNameMatches { get; set; }
        public string DescriptionMatches { get; set; }
        public string PrerequisiteMatches { get; set; }
        public string DepartmentMatches { get; set; }
        public string RawMatches { get; set; }
    }
}
