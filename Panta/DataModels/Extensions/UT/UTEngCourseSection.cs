using Panta.Indexing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Panta.DataModels.Extensions.UT
{
    [Serializable]
    public class UTEngCourseSection : UTCourseSection
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
                if (UTEngCourseSectionTime.TryParseRawTime(value, out time))
                {
                    this.ParsedTime = time;
                }
                else
                {
                    if (String.IsNullOrEmpty(value))
                    {
                        throw new ArgumentException("Cannot parse time: " + value);
                    }
                }
            }
        }
        public override string Name
        {
            get
            {
                return base.Name;
            }
            set
            {
                base.Name = value;
                this.IsLecture = value.StartsWith("LEC");
            }
        }
    }
}
