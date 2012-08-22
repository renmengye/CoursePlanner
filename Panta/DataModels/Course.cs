using System;
using System.Linq;
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

        /// <summary>
        /// Put all the properties into a form with prefix and root
        /// </summary>
        /// <returns>A list of prefixes and roots</returns>
        protected IEnumerable<IndexString> GetIndexStrings()
        {
            // Doesn't include strings in course sections
            List<IndexString> strings = new List<IndexString>();
            strings.Add(new IndexString("id:", this.ID.ToString()));
            strings.Add(new IndexString("code:", this.Code));
            strings.Add(new IndexString("name:", this.Name));
            strings.Add(new IndexString("sems:", this.Semester));
            strings.Add(new IndexString(null, this.Description));
            if(this.Prerequisites.FirstOrDefault()!=null) strings.Add(new IndexString("preq:", String.Join(" ", this.Prerequisites)));
            if (this.Exclusions.FirstOrDefault() != null) strings.Add(new IndexString("excl:", String.Join(" ", this.Exclusions)));
            if(!String.IsNullOrEmpty(this.BreadthRequirement)) strings.Add(new IndexString("bred:", this.BreadthRequirement));
            if (!String.IsNullOrEmpty(this.DistributionRequirement)) strings.Add(new IndexString("dist:", this.DistributionRequirement));
            return strings;
        }

        /// <summary>
        /// Parse/Split the roots and store them with prefixes and without prefixes
        /// </summary>
        /// <returns>A list of strings ready to be written into indexes</returns>
        public IEnumerable<string> GetSplittedIndexStrings()
        {
            List<string> results = new List<string>();
            foreach (IndexString istring in this.GetIndexStrings())
            {
                results.AddRange(istring.ToSplittedStrings());
            }
            foreach (CourseSection section in this.Sections.Values)
            {
                results.AddRange(section.GetSplittedIndexStrings());
            }
            return results;
        }
    }
}
