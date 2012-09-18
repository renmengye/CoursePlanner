using Panta.DataModels;
using Panta.DataModels.Extensions.UT;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Panta.Fetchers.Extensions.UT
{
    public class UTArtsciCourseInfoFetcher : WebpageItemFetcher<UTCourse>
    {
        public UTArtsciCourseInfoFetcher(string url) : base(url) { }

        private static Regex AngleRegex;
        private static Regex CircleRegex;
        private static Regex CodeRegex;
        private static Regex CourseRegex;

        static UTArtsciCourseInfoFetcher()
        {
            AngleRegex = new Regex("<[^>]+>", RegexOptions.Compiled);
            CircleRegex = new Regex("[\u0020]*[\u0028][^\u0029]*[\u0029][\u0020]*", RegexOptions.Compiled);
            CodeRegex = new Regex("(?<code>[A-Z]{3}[0-9]{3})(?<prefix>[HY][1])", RegexOptions.Compiled);
            CourseRegex = new Regex(@"<tr>.+?</tr>", RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Compiled);
        }

        public override IEnumerable<UTCourse> FetchItems()
        {
            List<UTCourse> results = new List<UTCourse>();

            if (this.Content == null) return results;

            this.Content = this.Content.Replace("\r\n", String.Empty);

            MatchCollection matches = CourseRegex.Matches(this.Content);

            // Used to accumulating course meet times
            string tempTime = "";
            string tempLocation = "";

            foreach (Match match in matches)
            {
                string line = match.Value;
                line = line.Replace("</td>", "|");
                line = AngleRegex.Replace(line, String.Empty);
                line = CircleRegex.Replace(line, String.Empty);

                string[] properties = line.Split('|');
                if (properties.Length < 8) continue;

                Match codeMatch = CodeRegex.Match(properties[0]);
                string code = codeMatch.Groups["code"].ToString();
                string prefix = codeMatch.Groups["prefix"].ToString();

                string semester = properties[1].Trim(' ');
                string name = properties[2].Trim(' ');
                string section = properties[3].Trim(' ');
                string wait = properties[4].Trim(' ');
                string time = properties[5].Replace(" ", "").Replace(",", "");
                string location = properties[6].Replace(" ", "");
                string instructor = properties[7].Trim(' ').Replace("&nbsp;", "");

                CourseSectionTime ptime;
                string matchedLocation = "";
                if (UTCourseSectionTime.TryParseRawTime(time, out ptime))
                {
                    if (!location.Equals("&nbsp;"))
                    {
                        matchedLocation = location;
                        for (int i = 1; i < ptime.MeetTimes.Count(); i++)
                        {
                            matchedLocation = String.Join(" ", matchedLocation, location);
                        }
                    }
                }

                // For some exceptions, section time is not written in one string, need to accumulating meet times
                if (section.Equals("&nbsp;"))
                {
                    tempTime += time;
                    tempLocation = String.Join(" ", tempLocation, matchedLocation);
                }
                else
                {
                    CourseSection courseSection = new UTCourseSection()
                    {
                        Name = section,
                        WaitList = wait.Equals("Y"),
                        Instructor = instructor,
                        Time = time,
                        Location = location
                    };

                    UTCourse lastCourse = null;

                    if (results.Count > 0)
                    {
                        lastCourse = results.Last<UTCourse>();

                        // Update the last tempTime and tempLocation
                        CourseSection lastSection = lastCourse.Sections.Last<CourseSection>();
                        lastSection.Time = tempTime;
                        lastSection.Location = tempLocation;
                    }

                    tempTime = time;
                    tempLocation = matchedLocation;

                    if (lastCourse != null)
                    {
                        // Only need to add a section
                        if (!codeMatch.Success)
                        {
                            if (results.Count > 0)
                            {
                                lastCourse.Sections.Add(courseSection);
                                Console.Write(" {0} ", courseSection.Name);
                                continue;
                            }
                        }
                    }

                    // Construct a course
                    UTCourse course = new UTCourse()
                    {
                        Code = code,
                        Name = name,
                        Semester = semester,
                        SemesterPrefix = prefix
                    };
                    course.Sections.Add(courseSection);
                    results.Add(course);
                    Console.Write("{0}Course: {1} {2}", Environment.NewLine, course.Abbr, courseSection.Name);
                }
                // Commit the last change to the last section
                CourseSection lastSec = results.Last<UTCourse>().Sections.Last<CourseSection>();
                lastSec.Time = tempTime;
                lastSec.Location = tempLocation;
            }
            return results;
        }
    }
}
