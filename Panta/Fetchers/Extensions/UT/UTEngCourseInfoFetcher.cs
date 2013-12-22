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

namespace Panta.Fetchers.Extensions.UT
{
    public class UTEngCourseInfoFetcher : WebpageItemFetcher<UTCourse>
    {
        public UTEngCourseInfoFetcher(string url) : base(url) { }

        private static Regex AngleRegex;
        private static Regex CircleRegex;
        private static Regex CodeRegex;
        private static Regex DepartmentRegex;
        private static Regex CourseRegex;

        static UTEngCourseInfoFetcher()
        {
            AngleRegex = new Regex("<[^>]+>", RegexOptions.Compiled);
            CircleRegex = new Regex("[\u0020]*[\u0028][^\u0029]*[\u0029][\u0020]*", RegexOptions.Compiled);
            CodeRegex = new Regex("(?<code>[A-Z]{3}[0-9]{3})(?<prefix>[HY][1])(?<semester>[FSY])", RegexOptions.Compiled);
            DepartmentRegex = new Regex("<table.+?</table>", RegexOptions.Compiled);
            CourseRegex = new Regex(@"<tr>.+?</tr>", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        }

        public override IEnumerable<UTCourse> FetchItems()
        {
            List<UTCourse> results = new List<UTCourse>();

            if (this.Content == null) return results;
            this.Content = this.Content.Replace("\r", String.Empty);
            this.Content = this.Content.Replace("\n", String.Empty);

            MatchCollection matches = DepartmentRegex.Matches(this.Content);

            Console.WriteLine("hi");

            Parallel.ForEach<Match>(matches.Cast<Match>(), new ParallelOptions() { MaxDegreeOfParallelism = Environment.ProcessorCount }, delegate(Match match)
            //foreach (Match match in matches)
            {
                List<UTCourse> partialResults = new List<UTCourse>();
                MatchCollection courseMatches = CourseRegex.Matches(match.Value);


                UTCourse lastCourse = null;
                CourseSection lastSection = null;
                // Accumulating course meet times
                string tempTime = "";
                string tempLocation = "";

                foreach (Match courseMatch in courseMatches)
                {
                    string line = courseMatch.Value;
                    line = line.Replace("</td>", "|");
                    line = AngleRegex.Replace(line, String.Empty);
                    line = CircleRegex.Replace(line, String.Empty);

                    string[] properties = line.Split('|');
                    if (properties.Length < 10) continue;

                    Match codeMatch = CodeRegex.Match(properties[0]);
                    string code = codeMatch.Groups["code"].ToString();
                    string prefix = codeMatch.Groups["prefix"].ToString();
                    string semester = codeMatch.Groups["semester"].ToString();

                    string section = properties[1].Replace(" ", "");
                    string meet = properties[2].Trim(' ');
                    string day = properties[3].Trim(' ');
                    string start = properties[4].Trim(' ').Replace("&nbsp", "");
                    string finish = properties[5].Trim(' ').Replace("&nbsp", "");
                    string location = properties[6].Trim(' ').Replace("&nbsp", "");
                    string notes = properties[7].Trim(' ').Replace("&nbsp", "");
                    string instructor = properties[8].Trim(' ').Replace("&nbsp", "");

                    string time = String.Join(" ", day, start, finish);

                    // Cumulating meet to the previous section
                    if (section.Equals("&nbsp"))
                    {
                        // Avoid duplication
                        if (!tempTime.Contains(time))
                        {
                            tempTime = String.Join(" ", tempTime, time);
                            tempLocation = String.Join(" ", tempLocation, location);
                        }
                    }
                    else
                    {
                        CourseSection courseSection = new UTEngCourseSection()
                        {
                            Name = section,
                            Instructor = instructor,
                            Time = time,
                            Location = location
                        };

                        if (partialResults.Count > 0)
                        {
                            lastCourse = partialResults.Last<UTCourse>();

                            // Update the last tempTime and tempLocation
                            lastSection = lastCourse.Sections.Last<CourseSection>();
                            lastSection.Time = tempTime;
                            lastSection.Location = tempLocation;
                        }

                        tempTime = time;
                        tempLocation = location;

                        if (lastCourse != null)
                        {
                            // Only need to add a section
                            if (lastCourse.Abbr.Equals(code + prefix + semester))
                            {
                                lastCourse.Sections.Add(courseSection);
                                continue;
                            }
                        }

                        // Construct a course
                        UTCourse course = new UTCourse()
                        {
                            Code = code,
                            Semester = semester,
                            SemesterPrefix = prefix,
                            Campus = "UTSG"
                        };
                        course.Sections.Add(courseSection);

                        partialResults.Add(course);

                        Console.WriteLine("Engineering Course: {0}", course.Abbr);
                    }
                }

                if (partialResults.Count > 0)
                {
                    // Commit the last change to the last section
                    lastSection = partialResults.Last<UTCourse>().Sections.Last<CourseSection>();
                    lastSection.Time = tempTime;
                    lastSection.Location = tempLocation;
                }

                lock (this)
                {
                    results.AddRange(partialResults);
                }
            });
            return results;
        }
    }
}
