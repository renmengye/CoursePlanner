using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using Panta.DataModels;
using Panta.DataModels.Extensions.UT;

namespace Panta.Fetchers
{
    public class UTArtsciCourseInfoFetcher : IItemFetcher<UTCourse>
    {
        public string Url { get; set; }

        public UTArtsciCourseInfoFetcher(string url)
        {
            this.Url = url;
        }

        public IEnumerable<UTCourse> FetchItems()
        {
            List<UTCourse> results = new List<UTCourse>();
            WebClient client = new WebClient();
            string content;

            try
            {
                content = client.DownloadString(this.Url);
            }
            catch (WebException ex)
            {
                ex.Source = "Unable to fetch: " + Url;
                Trace.WriteLine(ex.ToString());
                return results;
            }

            content = content.Replace("\r\n", String.Empty);
            Regex courseRegex = new Regex(@"<tr>.+?</tr>", RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Compiled);
            MatchCollection matches = courseRegex.Matches(content);
            foreach (Match match in matches)
            {
                string line = match.Value;
                line = line.Replace("</td>", "|");

                Regex angleRegex = new Regex("<[^>]+>", RegexOptions.Compiled);
                Regex circleRegex = new Regex("[\u0020]*[\u0028][^\u0029]*[\u0029][\u0020]*", RegexOptions.Compiled);
                line = angleRegex.Replace(line, String.Empty);
                line = circleRegex.Replace(line, String.Empty);

                string[] properties = line.Split('|');

                if (properties.Length < 8) continue;

                Regex meetingRegex = new Regex("[A-Z][0-9]{4}", RegexOptions.Compiled);
                if (!meetingRegex.IsMatch(properties[3])) continue;

                Regex codeRegex = new Regex("(?<abbr>[A-Z]{3}[0-9]{3})(?<prefix>[HY][1])", RegexOptions.Compiled);
                Match codeMatch = codeRegex.Match(properties[0]);
                string abbr = codeMatch.Groups["abbr"].ToString();
                string prefix = codeMatch.Groups["prefix"].ToString();

                string semester = properties[1].TrimStart(' ').TrimEnd(' ');
                string name = properties[2].TrimStart(' ').TrimEnd(' ');
                string section = properties[3].TrimStart(' ').TrimEnd(' ');
                string wait = properties[4].TrimStart(' ').TrimEnd(' ');
                string time = properties[5].TrimStart(' ').TrimEnd(' ');
                string location = properties[6].Replace("&nbsp;", "").Replace(" ","");
                string instructor = properties[7].TrimStart(' ').TrimEnd(' ').Replace("&nbsp;", "");

                CourseSection courseSection = new UTCourseSection()
                {
                    Name = section,
                    WaitList = wait.Equals("Y"),
                    Time = time,
                    Location = location,
                    Instructor = instructor
                };

                // Only need to add a section
                if (!codeMatch.Success)
                {
                    if (results.Count > 0)
                    {
                        UTCourse lastCourse = results.Last<UTCourse>();
                        lastCourse.Sections.Add(courseSection);
                        Console.Write(" {0} ", courseSection.Name);
                    }
                }
                // Need to create a course
                else
                {
                    UTCourse course = new UTCourse()
                    {
                        Abbr = abbr,
                        Name = name,
                        Semester = semester,
                        SemesterPrefix = prefix
                    };
                    course.Sections.Add(courseSection);
                    results.Add(course);
                    Console.Write("\nCourse: {0} {1}", course.Abbr, courseSection.Name);
                }
            }
            return results;
        }
    }
}
