using Panta.Indexing;
using System;
using System.Collections.Generic;

namespace Panta.DataModels.Extensions.UT
{
    [Serializable]
    public class UTCourseSection : CourseSection
    {
        private string _time;
        public override string Time
        {
            get
            {
                return this._time;
            }
            set
            {
                this._time = value;
                CourseSectionTime time;
                if (UTCourseSectionTime.TryParseRawTime(value.ToUpperInvariant().Replace(",", "").Replace(" ", ""), out time))
                {
                    this.ParsedTime = time;
                }
                else
                {
                    throw new ArgumentException("Cannot parse time: " + value);
                }
            }
        }
        public virtual CourseSectionTime ParsedTime { get; protected set; }
        public override string Name
        {
            get
            {
                return base.Name;
            }
            set
            {
                base.Name = value;
                this.IsLecture = value[0] == 'L';
            }
        }
        public bool IsLecture { get; protected set; }
        public bool WaitList { get; set; }

        protected override IList<IndexString> GetIndexStrings()
        {
            IList<IndexString> strings = base.GetIndexStrings();

            if (!String.IsNullOrEmpty(this.Location)) strings.Add(new IndexString("loc:", this.Location));
            
            // Only index the time if the section is a lecture section
            if (this.IsLecture)
            {
                if (!String.IsNullOrEmpty(this.ParsedTime.ToString())) strings.Add(new IndexString("time:", this.ParsedTime.ToString()));
            }
            return strings;
        }
    }
}
