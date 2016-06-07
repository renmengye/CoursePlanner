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
                        throw new ArgumentException(string.Format("Unknown semester: {}", value));
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
        public string Postrequisites { get; set; }

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
        public string Categories { get; set; }

        [DataMember]
        public string Campus { get; set; }

        [DataMember]
        public UTCourseEvaluation Evaluation { get; set; }

        protected override IList<IndexString> GetIndexStrings()
        {
            IList<IndexString> strings = base.GetIndexStrings();
            if (!String.IsNullOrEmpty(this.SemesterDetail)) strings.Add(new IndexString("sems:", this.SemesterDetail));
            if (!String.IsNullOrEmpty(this.Prerequisites)) strings.Add(new IndexString("preq:", this.Prerequisites));
            if (!String.IsNullOrEmpty(this.Postrequisites)) strings.Add(new IndexString("post:", this.Postrequisites));
            if (!String.IsNullOrEmpty(this.Corequisites)) strings.Add(new IndexString("creq:", this.Prerequisites));
            if (!String.IsNullOrEmpty(this.Exclusions)) strings.Add(new IndexString("excl:", this.Exclusions));
            if (!String.IsNullOrEmpty(this.BreadthRequirement)) strings.Add(new IndexString("bred:", this.BreadthRequirement));
            if (!String.IsNullOrEmpty(this.DistributionRequirement)) strings.Add(new IndexString("dist:", this.DistributionRequirement));
            if (!String.IsNullOrEmpty(this.Program)) strings.Add(new IndexString("prog:", this.Program));
            if (!String.IsNullOrEmpty(this.Faculty)) strings.Add(new IndexString("fac:", this.Faculty));
            if (!String.IsNullOrEmpty(this.Campus)) strings.Add(new IndexString("camp:", this.Campus));
            if (!String.IsNullOrEmpty(this.Categories))
            {
                foreach (string category in this.Categories.Split(','))
                {
                    strings.Add(new IndexString("cat:", category));
                }
            }

            return strings;
        }

        public void AddCategory(string category)
        {
            if (String.IsNullOrEmpty(this.Categories))
            {
                this.Categories = category;
            }
            else
            {
                this.Categories = String.Join(",", this.Categories, category);
            }
        }
    }
}
