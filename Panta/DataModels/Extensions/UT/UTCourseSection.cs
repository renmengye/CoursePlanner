using Panta.Indexing;
using System;
using System.Collections.Generic;

namespace Panta.DataModels.Extensions.UT
{
    [Serializable]
    public class UTCourseSection : CourseSection
    {
        public override string Time
        {
            get
            {
                return base.Time;
            }
            set
            {
                base.Time = value;
                CourseSectionTime time;
                if (UTCourseSectionTime.TryParseRawTime(value.ToUpperInvariant(), out time))
                {
                    this.ParsedTime = time;
                }
                else
                {
                    throw new ArgumentException("Cannot parse time");
                }
            }
        }
        public CourseSectionTime ParsedTime { get; private set; }
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
        public bool IsLecture { get; private set; }
        public bool WaitList { get; set; }

        protected override IList<IndexString> GetIndexStrings()
        {
            IList<IndexString> strings = base.GetIndexStrings();

            // Only index the time if the section is a lecture section
            if (this.IsLecture)
            {
                if (!String.IsNullOrEmpty(this.Time)) strings.Add(new IndexString("time:", this.Time));
                strings.Add(new IndexString("dtime:", this.ParsedTime.ToString()));
            }
            return strings;
        }
    }
}
