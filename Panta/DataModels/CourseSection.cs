using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Panta.Indexing;
using System.Runtime.Serialization;
using Panta.DataModels.Extensions.UT;

namespace Panta.DataModels
{
    [DataContract]
    [KnownType(typeof(UTCourseSection))]
    [Serializable]
    public class CourseSection
    {
        public virtual string Time { get; set; }

        [DataMember]
        public virtual string Name { get; set; }

        [DataMember]
        public string Instructor { get; set; }

        [DataMember]
        public string Location { get; set; }

        /// <summary>
        /// Put all the properties into a form with prefix and root
        /// </summary>
        /// <returns>A list of prefixes and roots</returns>
        protected virtual IList<IndexString> GetIndexStrings()
        {
            List<IndexString> strings = new List<IndexString>();
            if (!String.IsNullOrEmpty(this.Name)) strings.Add(new IndexString("name:", this.Name));
            if (!String.IsNullOrEmpty(this.Instructor)) strings.Add(new IndexString("inst:", this.Instructor));
            if (!String.IsNullOrEmpty(this.Location)) strings.Add(new IndexString("loc:", this.Location));

            return strings;
        }

        /// <summary>
        /// Parse/Split the roots and store them with prefixes and without prefixes
        /// </summary>
        /// <returns>A list of strings ready to be written into indexes</returns>
        public IEnumerable<string> GetSplittedIndexStrings()
        {
            IEnumerable<IndexString> indexStrings = GetIndexStrings();
            List<string> results = new List<string>();
            foreach (IndexString istring in indexStrings)
            {
                results.AddRange(istring.ToSplittedStrings());
            }
            return results;
        }
    }
}
