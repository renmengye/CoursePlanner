using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;

namespace Panta.DataModels
{
    [DataContract]
    [Serializable]
    public struct CourseSectionTime
    {
        public IEnumerable<CourseSectionTimeSpan> MeetTimes;

        public CourseSectionTime(IEnumerable<CourseSectionTimeSpan> times)
        {
            this.MeetTimes = times;
        }

        /// <summary>
        /// Concatenate each timespan with space
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (MeetTimes != null)
            {
                return String.Join(" ", MeetTimes.Select<CourseSectionTimeSpan, string>(span => span.ToString()));
            }
            else
            {
                return String.Empty;
            }
        }
    }

    [DataContract]
    [Serializable]
    public struct CourseSectionTimeSpan
    {
        public DayOfWeek Day;

        // An integer from 0 to 95 (counts every quarter)
        public byte Start;
        public byte End;

        public CourseSectionTimeSpan(DayOfWeek day, byte start, byte end)
        {
            if (start < 0 || start > 95) throw new ArgumentException("Invalid start time");
            if (end < 0 || end > 95) throw new ArgumentException("Invalid end time");
            this.Day = day;
            this.Start = start;
            this.End = end;
        }

        /// <summary>
        /// Build a custom string representing a course section
        /// </summary>
        /// <returns>In the form Monday 10:30-13:30</returns>
        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(Day.ToString() + " ");
            builder.Append(String.Format("{0}:{1}", Start / 4, ToMinute(Start)));
            builder.Append("-");
            builder.Append(String.Format("{0}:{1}", End / 4, ToMinute(End)));

            return builder.ToString();
        }

        private string ToMinute(byte time)
        {
            switch (time % 4)
            {
                case 0:
                    return "00";
                case 1:
                    return "15";
                case 2:
                    return "30";
                case 3:
                    return "45";
            }
            return String.Empty;
        }
    }
}
