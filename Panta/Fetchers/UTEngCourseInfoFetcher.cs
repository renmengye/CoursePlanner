using Panta.DataModels;
using Panta.DataModels.Extensions.UT;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Panta.Fetchers
{
    public class UTEngCourseInfoFetcher : IItemFetcher<UTCourse>
    {
        public string Url
        {
            get;
            set;
        }

        public UTEngCourseInfoFetcher(string url)
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

            content = content.Replace("\r", String.Empty);
            content = content.Replace("\n", String.Empty);
            Regex courseRegex = new Regex(@"<tr>.+?</tr>", RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Compiled);
            MatchCollection matches = courseRegex.Matches(content);

            // Accumulating course meet times
            string tempTime = "";
            string tempLocation = "";

            foreach (Match match in matches)
            {
                string line = match.Value;
                line = line.Replace("</td>", "|");

                Regex angleRegex = new Regex("<[^>]+>", RegexOptions.Compiled);
                Regex circleRegex = new Regex("[\u0020]*[\u0028][^\u0029]*[\u0029][\u0020]*", RegexOptions.Compiled);
                line = angleRegex.Replace(line, String.Empty);
                line = circleRegex.Replace(line, String.Empty);

                string[] properties = line.Split('|');

                if (properties.Length < 10) continue;

                Regex codeRegex = new Regex("(?<abbr>[A-Z]{3}[0-9]{3})(?<prefix>[HY][1])(?<semester>[FSY])", RegexOptions.Compiled);
                Match codeMatch = codeRegex.Match(properties[0]);
                string abbr = codeMatch.Groups["abbr"].ToString();
                string prefix = codeMatch.Groups["prefix"].ToString();
                string semester = codeMatch.Groups["semester"].ToString();

                string section = properties[1].Replace(" ", "");
                string meet = properties[2].TrimStart(' ').TrimEnd(' ');
                string day = properties[3].TrimStart(' ').TrimEnd(' ');

                string start = properties[4].TrimStart(' ').TrimEnd(' ').Replace("&nbsp", "");
                string finish = properties[5].TrimStart(' ').TrimEnd(' ').Replace("&nbsp", "");
                string location = properties[6].TrimStart(' ').TrimEnd(' ').Replace("&nbsp", "");
                string notes = properties[7].TrimStart(' ').TrimEnd(' ').Replace("&nbsp", "");
                string instructor = properties[8].TrimStart(' ').TrimEnd(' ').Replace("&nbsp", "");

                string time = String.Join(" ", day, start, finish);

                // Cumulating meet to the previous section
                if (section.Equals("&nbsp"))
                {
                    tempTime = String.Join(" ", tempTime, time);
                    tempLocation = String.Join("; ", tempLocation, location);
                }
                else
                {
                    CourseSection courseSection = new UTEngCourseSection()
                    {
                        Name = section,
                        Instructor = instructor
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
                    tempLocation = location;

                    if (lastCourse != null)
                    {
                        // Only need to add a section
                        if (lastCourse.FullAbbr.Equals(abbr + prefix + semester))
                        {
                            lastCourse.Sections.Add(courseSection);
                            continue;
                        }
                    }

                    // Construct a course
                    UTCourse course = new UTCourse()
                    {
                        Abbr = abbr,
                        Semester = semester,
                        SemesterPrefix = prefix
                    };
                    course.Sections.Add(courseSection);
                    results.Add(course);
                    Console.WriteLine("Engineering Course: {0}", course.Abbr);
                }
            }

            return results;
        }
    }
}
