using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Panta.DataModels.Extensions.UT
{
    public class UTSCCourseSectionTime
    {
        public static bool TryParseRawTime(string time, out CourseSectionTime result)
        {
            if (time == "TBA")
            {
                result = new CourseSectionTime()
                {
                    TBA = true
                };
                return true;
            }
            Regex regex = new Regex("((?<day>(mo)|(tu)|(we)|(th)|(fr)) (?<start>[0-9][0-9]?:[0-9]{2}) (?<end>[0-9][0-9]?:[0-9]{2}))+", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            MatchCollection collection = regex.Matches(time);
            result = new CourseSectionTime();
            List<CourseSectionTimeSpan> meetTimes = new List<CourseSectionTimeSpan>();
            if (collection.Count > 0)
            {
                foreach (Match match in collection)
                {
                    CourseSectionTimeSpan span;
                    DayOfWeek day;
                    switch (match.Groups["day"].ToString().ToLowerInvariant())
                    {
                        case "mo":
                            day = DayOfWeek.Monday;
                            break;
                        case "tu":
                            day = DayOfWeek.Tuesday;
                            break;
                        case "we":
                            day = DayOfWeek.Wednesday;
                            break;
                        case "th":
                            day = DayOfWeek.Thursday;
                            break;
                        case "fr":
                            day = DayOfWeek.Friday;
                            break;
                        default:
                            return false;
                    }
                    span.Day = day;

                    byte start, end;
                    if (!UTCourseSectionTimeSpan.TryParseTimeSpanInt(match.Groups["start"].ToString(), out start)) return false;
                    if (!UTCourseSectionTimeSpan.TryParseTimeSpanInt(match.Groups["end"].ToString(), out end)) return false;

                    meetTimes.Add(new CourseSectionTimeSpan(day, start, end));

                }
                result.MeetTimes = meetTimes;
                return true;
            }
            return false;
        }
    }
}
