using System;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Panta.DataModels.Extensions.UT
{
    [Serializable]
    public static class UTCourseSectionTime
    {
        private static Regex rawTimeRegex;
        private static Regex timeStringRegex;

        static UTCourseSectionTime()
        {
            rawTimeRegex = new Regex("^(?<span>[A-Z][0-9:-]*)+$", RegexOptions.Compiled);
            timeStringRegex = new Regex("((?<day>(monday)|(tuesday)|(wednesday)|(thursday)|(friday)) (?<start>[0-9][0-9]?:[0-9]{2})-(?<end>[0-9][0-9]?:[0-9]{2}))+", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        }

        public static bool TryParseRawTime(string raw, out CourseSectionTime time)
        {
            time = new CourseSectionTime();
            if (raw.Equals("TBA")) return true;

            if (!rawTimeRegex.IsMatch(raw)) throw new ArgumentException("Fail to parse the time: " + raw);

            List<CourseSectionTimeSpan> spans = new List<CourseSectionTimeSpan>();

            foreach (Capture capture in rawTimeRegex.Match(raw).Groups["span"].Captures)
            {
                CourseSectionTimeSpan span;
                string rawSpan = capture.ToString();
                if (!UTCourseSectionTimeSpan.TryParseRawTimeSpan(rawSpan, out span)) return false;

                // Correct the abbreviation form, e.g. TF9 => T9F9
                if (span.Start != 0 || span.End != 0)
                {
                    for (int i = 0; i < spans.Count; i++)
                    {
                        CourseSectionTimeSpan prevSpan = spans[i];
                        if (prevSpan.Start == 0 && prevSpan.End == 0)
                        {
                            prevSpan.Start = span.Start;
                            prevSpan.End = span.End;
                        }
                        spans[i] = prevSpan;
                    }
                }

                spans.Add(span);
            }

            time.MeetTimes = spans;
            return true;
        }

        public static bool TryParseTimeString(string time, out CourseSectionTime result)
        {
            MatchCollection collection = timeStringRegex.Matches(time);
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
                        case "monday":
                            day = DayOfWeek.Monday;
                            break;
                        case "tuesday":
                            day = DayOfWeek.Tuesday;
                            break;
                        case "wednesday":
                            day = DayOfWeek.Wednesday;
                            break;
                        case "thursday":
                            day = DayOfWeek.Thursday;
                            break;
                        case "friday":
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


    [Serializable]
    public static class UTCourseSectionTimeSpan
    {
        private static Regex rawSpanRegex;

        static UTCourseSectionTimeSpan()
        {
            rawSpanRegex = new Regex("^(?<day>[A-Z])" +
                "(?:" +
                "(?<start>[0-9]+(?:[:][0-9]{2})?)" +
                "(?:[-]" +
                "(?<end>[0-9]+(?:[:][0-9]{2})?)" +
                ")?)?$", RegexOptions.Compiled);
        }

        /// <summary>
        /// Convert a raw time string to a standard integer accepted by CourseTimeSpan constructor
        /// </summary>
        /// <param name="rawTime">Raw time string in the form 11:30 or 11</param>
        /// <returns>11:30 returns 46</returns>
        public static bool TryParseTimeSpanInt(string rawTime, out byte result)
        {
            string[] half = rawTime.Split(':');

            if (!Byte.TryParse(half[0], out result)) return false;

            result *= 4;

            if (half.Length > 1)
            {
                if (half.Length > 2) return false;
                switch (half[1])
                {
                    case "00":
                        break;
                    case "15":
                        result += 1;
                        break;
                    case "30":
                        result += 2;
                        break;
                    case "45":
                        result += 3;
                        break;
                    default:
                        return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Parse the time format on UT artsci to CourseTimeSpan format
        /// </summary>
        /// <param name="rawSpan">e.g. "M2-4"</param>
        /// <returns>CourseTimeSpan Monday 28 32</returns>
        public static bool TryParseRawTimeSpan(string rawSpan, out CourseSectionTimeSpan span)
        {
            span = new CourseSectionTimeSpan();

            if (!rawSpanRegex.IsMatch(rawSpan)) throw new ArgumentException("Fail to parse the span: " + rawSpan);
            Match match = rawSpanRegex.Match(rawSpan);

            // Process day
            DayOfWeek day;
            switch (match.Groups["day"].ToString())
            {
                case "M":
                    day = DayOfWeek.Monday;
                    break;
                case "T":
                    day = DayOfWeek.Tuesday;
                    break;
                case "W":
                    day = DayOfWeek.Wednesday;
                    break;
                case "R":
                    day = DayOfWeek.Thursday;
                    break;
                case "F":
                    day = DayOfWeek.Friday;
                    break;
                default:
                    return false;
            }
            span.Day = day;

            // Process time
            if (!String.IsNullOrEmpty(match.Groups["start"].ToString()))
            {
                byte startTime, endTime;
                if (!TryParseTimeSpanInt(match.Groups["start"].ToString(), out startTime)) return false;
                if (!String.IsNullOrEmpty(match.Groups["end"].ToString()))
                {
                    if (!TryParseTimeSpanInt(match.Groups["end"].ToString(), out endTime)) return false;
                }
                else
                {
                    endTime = (byte)(startTime + 4);
                    if (endTime == 48) endTime = 0;
                }
                span.Start = To24HourTime(startTime);
                span.End = To24HourTime(endTime, span.Start >= 48);
            }

            return true;
        }

        /// <summary>
        /// Convert a 12-hour time to 24-hour time
        /// </summary>
        /// <param name="time">A 12-hour time using integer 0-23</param>
        /// <returns>A 24-hour time using 0-47</returns>
        private static byte To24HourTime(byte time, bool fixAfternoon = false)
        {
            // Heuristics:
            // Number smaller than 8:00 are afternoon
            // Number bigger than 8:00 (inclusive) are morning
            if (time > 95 || time < 0) throw new ArgumentException("Invalid 12-hour time");
            if (time < 36 || fixAfternoon)
            {
                return (byte)(time + 48);
            }
            else
            {
                return time;
            }
        }
    }
}
