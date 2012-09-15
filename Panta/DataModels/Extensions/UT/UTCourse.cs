using Panta.Indexing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;


namespace Panta.DataModels.Extensions.UT
{
    [DataContract]
    [Serializable]
    public class UTCourse : Course
    {
        private string _semester;

        [DataMember]
        public string Semester
        {
            get { return _semester; }
            set
            {
                _semester = value;
                switch (value)
                {
                    case "Y":
                        SemesterDetail = "Year";
                        break;
                    case "F":
                        SemesterDetail = "Fall";
                        break;
                    case "S":
                        SemesterDetail = "Winter";
                        break;
                    default:
                        SemesterDetail = "Year";
                        break;
                }
            }
        }
        public string SemesterDetail { get; private set; }
        public string SemesterPrefix { get; set; }
        public override string Abbr { get { return this.Code + this.SemesterPrefix + this.Semester; } }
        public string Code { get; set; }

        [DataMember]
        public string Prerequisites { get; set; }

        [DataMember]
        public string Corequisites { get; set; }

        [DataMember]
        public string Exclusions { get; set; }

        [DataMember]
        public string DistributionRequirement { get; set; }

        [DataMember]
        public string BreadthRequirement { get; set; }
        public string Program { get; set; }
        public string Faculty { get; set; }

        protected override IList<IndexString> GetIndexStrings()
        {
            IList<IndexString> strings = base.GetIndexStrings();
            if (!String.IsNullOrEmpty(this.SemesterDetail)) strings.Add(new IndexString("sems:", this.SemesterDetail));
            if (!String.IsNullOrEmpty(this.Prerequisites)) strings.Add(new IndexString("preq:", this.Prerequisites));
            if (!String.IsNullOrEmpty(this.Corequisites)) strings.Add(new IndexString("creq:", this.Prerequisites));
            if (!String.IsNullOrEmpty(this.Exclusions)) strings.Add(new IndexString("excl:", this.Exclusions));
            if (!String.IsNullOrEmpty(this.BreadthRequirement)) strings.Add(new IndexString("bred:", this.BreadthRequirement));
            if (!String.IsNullOrEmpty(this.DistributionRequirement)) strings.Add(new IndexString("dist:", this.DistributionRequirement));
            if (!String.IsNullOrEmpty(this.Program)) strings.Add(new IndexString("prog:", this.Program));
            if (!String.IsNullOrEmpty(this.Faculty)) strings.Add(new IndexString("fac:", this.Faculty));

            return strings;
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder(base.ToString());
            foreach (CourseSection section in this.Sections)
            {
                builder.AppendLine("Section: "+section.Name+" Time: " + (section as UTCourseSection).ParsedTime.ToString());
            }
            builder.AppendLine("Semester: " + this.Semester);
            builder.AppendLine("Prerequisites: " + this.Prerequisites);
            builder.AppendLine("Corequisites: " + this.Corequisites);
            builder.AppendLine("Exclusions: " + this.Exclusions);
            builder.AppendLine("Breath: " + this.BreadthRequirement);
            builder.AppendLine("Distribution: " + this.DistributionRequirement);

            return builder.ToString();
        }
    }
}
