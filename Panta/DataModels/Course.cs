using System;
using System.Linq;
using System.Collections.Generic;
using Panta.Indexing;

namespace Panta.DataModels
{
    [Serializable]
    public class Course : IIndexable, IName
    {
        public uint ID { get; set; }
        private string _abbr;
        public virtual string Abbr { get { return this._abbr; } set { this._abbr = value; } }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Department { get; set; }

        public IList<CourseSection> Sections { get; set; }

        public Course()
        {
            this.Sections = new List<CourseSection>();
        }

        /// <summary>
        /// Put all the properties into a form with prefix and root
        /// </summary>
        /// <returns>A list of prefixes and roots</returns>
        protected virtual IList<IndexString> GetIndexStrings()
        {
            // Doesn't include strings in course sections
            IList<IndexString> strings = new List<IndexString>();
            strings.Add(new IndexString("id:", this.ID.ToString()));
            strings.Add(new IndexString("code:", this.Abbr));
            strings.Add(new IndexString("name:", this.Name));
            strings.Add(new IndexString("dep:", this.Department));
            strings.Add(new IndexString(null, this.Description));
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
            foreach (CourseSection section in this.Sections)
            {
                results.AddRange(section.GetSplittedIndexStrings());
            }
            return results;
        }
    }
}
