using Panta.Indexing;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Panta.DataModels.Extensions.UT
{
    [Serializable]
    public class UTCourse : Course
    {
        private string _semester;
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
                        SemesterDetail = "First Fall";
                        break;
                    case "S":
                        SemesterDetail = "Second Winter";
                        break;
                    default:
                        SemesterDetail = "Year";
                        break;
                }
            }
        }
        public string SemesterDetail { get; private set; }
        public string SemesterPrefix { get; set; }
        public string FullAbbr { get { return this.Abbr + this.SemesterPrefix + this.Semester; } }
        public IEnumerable<string> Prerequisites { get; set; }
        public IEnumerable<string> Exclusions { get; set; }
        public string DistributionRequirement { get; set; }
        public string BreadthRequirement { get; set; }

        public UTCourse()
            : base()
        {
            this.Prerequisites = new List<string>();
            this.Exclusions = new List<string>();
        }

        protected override IList<IndexString> GetIndexStrings()
        {
            IList<IndexString> strings = base.GetIndexStrings();
            if (!String.IsNullOrEmpty(this.SemesterDetail)) strings.Add(new IndexString("sems:", this.SemesterDetail));
            if (this.Prerequisites.FirstOrDefault() != null) strings.Add(new IndexString("preq:", String.Join(" ", this.Prerequisites)));
            if (this.Exclusions.FirstOrDefault() != null) strings.Add(new IndexString("excl:", String.Join(" ", this.Exclusions)));
            if (!String.IsNullOrEmpty(this.BreadthRequirement)) strings.Add(new IndexString("bred:", this.BreadthRequirement));
            if (!String.IsNullOrEmpty(this.DistributionRequirement)) strings.Add(new IndexString("dist:", this.DistributionRequirement));
            return strings;
        }
    }
}
