using System;
using System.Collections.Generic;
using Panta.Indexing;

namespace Panta.DataModels
{
    [Serializable]
    public class Course : IIndexable
    {
        public uint ID { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Semester { get; set; }

        // use section name as dictionary key
        public Dictionary<string, CourseSection> Sections { get; set; }

        public string Description { get; set; }
        public IEnumerable<string> Prerequisites { get; set; }
        public IEnumerable<string> Exclusions { get; set; }
        public string DistributionRequirement { get; set; }
        public string BreadthRequirement { get; set; }

        public Course()
        {
            this.Sections = new Dictionary<string, CourseSection>();
            this.Prerequisites = new List<string>();
            this.Exclusions = new List<string>();
        }

        public IEnumerable<IndexString> GetIndexStrings()
        {
            HashSet<IndexString> strings = new HashSet<IndexString>();
            strings.Add(new IndexString("id:", this.ID.ToString()));
            strings.Add(new IndexString("code:", this.Code));
            strings.Add(new IndexString("name:", this.Name));
            strings.Add(new IndexString("sems:", this.Semester));
            strings.Add(new IndexString(null, this.Description));
            strings.Add(new IndexString("preq:", String.Join(" ", this.Prerequisites)));
            strings.Add(new IndexString("excl:", String.Join(" ", this.Exclusions)));
            strings.Add(new IndexString("bred:", this.BreadthRequirement));
            strings.Add(new IndexString("dist:", this.DistributionRequirement));
            return strings;
        }
    }

    [Serializable]
    public class CourseSection
    {
        // Contains time, location, waitlist, instructor
        public string Time { get; set; }
        public string Name { get; set; }
        public string Location { get; set; }
        public bool WaitList { get; set; }
        public string Instructor { get; set; }
    }
}
