using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using Panta.DataModels;

namespace Panta.Formatters
{
    [Serializable]
    public class UTCourseFormatter : IWebpageFormatter<Course>
    {
        public string Url { get; set; }

        public UTCourseFormatter(string url)
        {
            this.Url = url;
        }

        public IEnumerable<Course> Read()
        {
            List<Course> results = new List<Course>();
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

                if (properties.Length < 4) continue;

                Regex meetingRegex = new Regex("[A-Z][0-9]{4}");
                if (!meetingRegex.IsMatch(properties[3])) continue;

                string code = properties[0].TrimStart(' ').TrimEnd(' ');
                string semester = properties[1].TrimStart(' ').TrimEnd(' ');
                string name = properties[2].TrimStart(' ').TrimEnd(' ');
                string section = properties[3].TrimStart(' ').TrimEnd(' ');
                string wait = properties[4].TrimStart(' ').TrimEnd(' ');
                string time = properties[5].TrimStart(' ').TrimEnd(' ');

                CourseSection courseSection = new CourseSection()
                {
                    Name = section,
                    WaitList = wait.Equals("Y"),
                    Time = time
                };

                // Only need to add a section
                if (code.Equals("&nbsp;") || String.IsNullOrEmpty(code))
                {
                    Course lastCourse = results.Last<Course>();
                    lastCourse.Sections.Add(courseSection.Name, courseSection);
                    Console.Write(" {0} ", courseSection.Name);
                }
                // Need to create a course
                else
                {
                    Course course = new Course()
                    {
                        Code = code,
                        Name = name,
                        Semester = semester
                    };
                    course.Sections.Add(courseSection.Name, courseSection);
                    results.Add(course);
                    Console.Write("\n{0} {1}", course.Code, courseSection.Name);
                }
            }
            return results;
        }
    }
}
