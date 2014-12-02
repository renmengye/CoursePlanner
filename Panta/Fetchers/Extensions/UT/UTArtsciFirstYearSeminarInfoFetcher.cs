using Panta.DataModels;
using Panta.DataModels.Extensions.UT;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace Panta.Fetchers.Extensions.UT
{
    public class UTArtsciFirstYearSeminarInfoFetcher : WebpageItemFetcher<UTCourse>
    {
        public UTArtsciFirstYearSeminarInfoFetcher() : base("http://www.artsandscience.utoronto.ca/ofr/timetable/winter/assem.html") { }

        private static Regex AngleRegex;
        private static Regex CircleRegex;
        private static Regex CodeRegex;
        private static Regex CourseRegex;

        static UTArtsciFirstYearSeminarInfoFetcher()
        {
            AngleRegex = new Regex("<[^>]+>", RegexOptions.Compiled);
            CircleRegex = new Regex("[\u0020]*[\u0028][^\u0029]*[\u0029][\u0020]*", RegexOptions.Compiled);
            CodeRegex = new Regex("(?<code>[A-Z]{3}[0-9]{3})(?<prefix>[HY][0-9])(?<semester>[FSY])", RegexOptions.Compiled);
            CourseRegex = new Regex(@"<tr>.+?</tr>", RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Compiled);
        }

        public override IEnumerable<UTCourse> FetchItems()
        {
            List<UTCourse> results = new List<UTCourse>();

            if (this.Content == null) return results;
            this.Content = this.Content.Replace("\n", String.Empty).Replace("\r", String.Empty);

            MatchCollection matches = CourseRegex.Matches(this.Content);

            // Used to accumulating course meet times
            string courseName = null;

            foreach (Match match in matches)
            {
                string line = match.Value;

                if (CodeRegex.Match(line).Success)
                {
                    courseName = CodeRegex.Match(line).Value;
                    continue;
                }
                else
                {
                    if (courseName == null) continue;

                    line = line.Replace("</td>", "|");
                    line = AngleRegex.Replace(line, String.Empty);
                    line = CircleRegex.Replace(line, String.Empty);
                    string[] properties = line.Split('|');

                    if (line.ToLower().Contains("cancel")) continue;
                    if (properties.Length < 5) continue;

                    UTCourse course = new UTCourse()
                    {
                        Code = CodeRegex.Match(courseName).Groups["code"].Value,
                        SemesterPrefix = CodeRegex.Match(courseName).Groups["prefix"].Value,
                        Semester = CodeRegex.Match(courseName).Groups["semester"].Value,
                        Sections = new List<CourseSection>(),
                        Name = properties[0].Trim(' ') + ": " + HttpUtility.HtmlDecode(properties[1].Trim(' ')),
                        Campus = "UTSG",
                        Faculty = "Artsci"
                    };

                    UTCourseSection section = new UTCourseSection()
                    {
                        Name = properties[0].Trim(' '),
                        Time = HttpUtility.HtmlDecode(properties[3]).Replace(" ", String.Empty).Trim(' ').Trim(' '),
                        Location = HttpUtility.HtmlDecode(properties[4]).Replace(" ", String.Empty).Trim(' '),
                        Instructor = HttpUtility.HtmlDecode(properties[5]).Replace(" ", String.Empty).Trim(' ')
                    };

                    // Accumulate to last course
                    if (course.Name == "&nbsp;:  ")
                    {
                        section.Name = results.Last().Sections.Last().Name;
                        results.Last().Sections.Add(section);
                    }
                    else
                    {
                        course.Sections.Add(section);
                        results.Add(course);
                        Console.WriteLine("Course: " + course.Abbr);
                    }
                }
            }
            return results;
        }
    }
}
