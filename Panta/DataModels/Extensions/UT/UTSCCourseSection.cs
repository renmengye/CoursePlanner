using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Panta.DataModels.Extensions.UT
{
    [DataContract]
    [Serializable]
    public class UTSCCourseSection : UTCourseSection
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
                if (value != null)
                {
                    this._time = value;
                    CourseSectionTime time;
                    if (UTSCCourseSectionTime.TryParseRawTime(value, out time))
                    {
                        this.ParsedTime = time;
                    }
                    else
                    {
                        this.Time = "TBA";
                    }
                }
                else
                {
                    this.Time = "TBA";
                }
            }
        }

        [DataMember]
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
