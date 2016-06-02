using Panta.DataModels;
using Panta.DataModels.Extensions.UT;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace Panta.Fetchers.Extensions.UTSC
{
    public class UTSCCourseInfoHtmlFetcher : IItemFetcher<UTCourse>
    {
        private static Regex AngleRegex, TableRegex, CodeRegex, SectionRegex;
        private string Content;

        static UTSCCourseInfoHtmlFetcher()
        {
            AngleRegex = new Regex("<[^>]+>", RegexOptions.Compiled);
            TableRegex = new Regex("<tr.+?</tr>", RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Compiled);
            CodeRegex = new Regex("[A-Z]{4}[0-9]{2}[HY]3[FSY]", RegexOptions.Compiled);
            SectionRegex = new Regex("(LEC|TUT)[0-9]+", RegexOptions.Compiled);
        }

        public UTSCCourseInfoHtmlFetcher(string url)
        {
            var request = (HttpWebRequest)WebRequest.Create(url);

            var postData = "sess=year";
            postData += "&course=DISPLAY_ALL";
            postData += "&submit=Display by Discipline";
            postData += "&course2=";
            var data = Encoding.ASCII.GetBytes(postData);

            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = data.Length;

            using (var stream = request.GetRequestStream())
            {
                stream.Write(data, 0, data.Length);
            }

            var response = (HttpWebResponse)request.GetResponse();
            this.Content = new StreamReader(response.GetResponseStream()).ReadToEnd();
        }

        public IEnumerable<UTCourse> FetchItems()
        {
            List<UTCourse> results = new List<UTCourse>();

            if (this.Content == null) return results;

            this.Content = this.Content.Replace("\r\n", String.Empty);
            this.Content = this.Content.Replace("\n", String.Empty);
            MatchCollection matches = TableRegex.Matches(this.Content);
            foreach (var match in matches)
            {
                var line = match.ToString();
                var codeMatch = CodeRegex.Match(line);
                var secMatch = SectionRegex.Match(line);
                if (codeMatch.Success && !line.Contains("<br>"))
                {
                    // New course
                    string courseName = AngleRegex.Replace(line, String.Empty);
                    UTCourse course = new UTCourse()
                    {
                        Code = codeMatch.Value.Substring(0, 6),
                        Name = courseName.Substring(12),
                        Semester = codeMatch.Value.Substring(8, 1),
                        SemesterPrefix = codeMatch.Value.Substring(6, 2),
                        Campus = "UTSC"
                    };
                    results.Add(course);
                    Console.Write("{0}Course: {1} ", Environment.NewLine, course.Abbr);
                }
                else if (secMatch.Success)
                {
                    // New section
                    if (results.Count > 0)
                    {
                        var lastCourse = results.Last();
                        line = line.Replace("</td>", "|");
                        line = AngleRegex.Replace(line, String.Empty);
                        var parts = line.Split('|');
                        if (parts.Length >= 6)
                        {
                            var sectionName = parts[0];
                            var sectionDayRaw = parts[1];
                            var sectionTimeStart = parts[2];
                            var sectionTimeEnd = parts[3];
                            var sectionLocation = parts[4].Replace(" ", String.Empty);
                            var sectionInstructor = parts[5];
                            UTSCCourseSection section = new UTSCCourseSection()
                            {
                                Name = sectionName,
                                Location = sectionLocation,
                                Instructor = sectionInstructor,
                                Time = String.Join(" ", sectionDayRaw, sectionTimeStart, sectionTimeEnd)
                            };
                            lastCourse.Sections.Add(section);
                            Console.Write(" {0} ", section.Name);
                        }
                    }
                }
                else if (line.Contains("MO") ||
                    line.Contains("TU") ||
                    line.Contains("WE") ||
                    line.Contains("TH") ||
                    line.Contains("FR"))
                {
                    // New meet time.
                    if (results.Count > 0)
                    {
                        var lastCourse = results.Last();
                        if (lastCourse.Sections.Count > 0)
                        {
                            var lastSection = lastCourse.Sections.Last();

                            line = line.Replace("</td>", "|");
                            line = AngleRegex.Replace(line, String.Empty);
                            var parts = line.Split('|');
                            if (parts.Length >= 6)
                            {
                                var sectionName = parts[0];
                                var sectionDayRaw = parts[1];
                                var sectionTimeStart = parts[2];
                                var sectionTimeEnd = parts[3];
                                var sectionLocation = parts[4].Replace(" ", String.Empty);
                                var sectionInstructor = parts[5];
                                lastSection.Time = String.Join(" ", lastSection.Time, sectionDayRaw, sectionTimeStart, sectionTimeEnd);
                                lastSection.Location = String.Join(" ", lastSection.Location, sectionLocation);
                            }
                        }
                    }
                }
                else
                {
                    continue;
                }
            }
            return results;
        }
    }
}
