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
    public class UTEngCourseDetailFetcher : WebpageItemFetcher<UTCourse>
    {
        public UTEngCourseDetailFetcher(string url) : base(url) { }

        private static Regex DepartmentRegex;
        private static Regex CourseRegex;
        private static Regex CourseDetailRegex;
        private static Regex AngleRegex;
        private static Regex CodeRegex;
        private static Regex ProgramRegex;

        static UTEngCourseDetailFetcher()
        {
            DepartmentRegex = new Regex("<a name=\"Course[0-9]+\"></a><h2>(?<department>[^<]*)</h2>(?<courses>.*?)(?=(<a name=\"Course)|(<div id=\"footer\">))", RegexOptions.Compiled);
            CourseRegex = new Regex("<a name=\"[A-Z]{3}[0-9]{3}[HY][0-9]\"></a>(?<detail>.*?)(?=(<a name=\")|($))", RegexOptions.Compiled);
            CourseDetailRegex = new Regex("<span class=\"strong\">(?<code>[A-Z]{3}[0-9]{3})(?<prefix>[HY][0-9])&nbsp;(?<semester>([FSY])|(F/S))</span></td><td>" +
                "<span class=\"strong\">(?<name>[^<]+)</span>.*?(?=<a href)" +
                "<a href[^>]+>(?<program>.*?)</table>" +
                "(?<description>[^<]*)", RegexOptions.Compiled);
            AngleRegex = new Regex("<[^>]+>", RegexOptions.Compiled);
            CodeRegex = new Regex("(?<code>[A-Z]{3}[0-9]{3})(?<prefix>[HY][1])", RegexOptions.Compiled);
            ProgramRegex = new Regex("[A-Z]{8,}", RegexOptions.Compiled);
        }

        public override IEnumerable<UTCourse> FetchItems()
        {
            List<UTCourse> results = new List<UTCourse>();

            if (this.Content == null) return results;

            this.Content = this.Content.Replace("\r", String.Empty);
            this.Content = this.Content.Replace("\n", String.Empty);

            MatchCollection matches = DepartmentRegex.Matches(this.Content);

            // Go through all the departments
            Parallel.ForEach<Match>(matches.Cast<Match>(), new ParallelOptions() { MaxDegreeOfParallelism = Environment.ProcessorCount }, delegate(Match match)
            //foreach (Match match in matches)
            {
                MatchCollection courseMatches = CourseRegex.Matches(match.Groups["courses"].ToString());
                string department = match.Groups["department"].ToString().Trim(' ');

                // Go through all the courses
                foreach (Match courseMatch in courseMatches)
                {
                    string text = courseMatch.Groups["detail"].ToString();

                    // Match basic info
                    Match detailMatch = CourseDetailRegex.Match(text);

                    // Match preq, creq etc.
                    text = text.Replace("<br>", "|");
                    text = AngleRegex.Replace(text, String.Empty);

                    string prerequisites = null;
                    string corequisites = null;
                    string exclusions = null;
                    string distribution = null;

                    string[] properties = text.Split('|');
                    if (properties.Length > 1)
                    {
                        for (int i = 1; i < properties.Length; i++)
                        {
                            if (properties[i].StartsWith("Prerequisite:"))
                            {
                                prerequisites = String.Join(" ", GetMatchedValues(CodeRegex.Matches(properties[i])));
                            }
                            else if (properties[i].StartsWith("Corequisite:"))
                            {
                                corequisites = String.Join(" ", GetMatchedValues(CodeRegex.Matches(properties[i])));
                            }
                            else if (properties[i].StartsWith("Exclusion:"))
                            {
                                exclusions = String.Join(" ", GetMatchedValues(CodeRegex.Matches(properties[i])));
                            }
                            else if (properties[i].StartsWith("Distribution Requirement Status:"))
                            {
                                distribution = properties[i].Substring("Distribution Requirement Status:".Length).Trim(' ');
                            }
                        }
                    }

                    string semester = detailMatch.Groups["semester"].ToString();
                    if (semester.Equals("F/S"))
                    {
                        lock (this)
                        {
                            results.Add(new UTCourse()
                            {
                                Code = detailMatch.Groups["code"].ToString(),
                                SemesterPrefix = detailMatch.Groups["prefix"].ToString(),
                                Semester = "F",
                                Name = detailMatch.Groups["name"].ToString().Trim(' '),
                                Description = detailMatch.Groups["description"].ToString().Trim(' '),
                                Prerequisites = prerequisites,
                                Corequisites = corequisites,
                                Exclusions = exclusions,
                                DistributionRequirement = distribution,
                                Program = String.Join(" ", GetMatchedValues(ProgramRegex.Matches(AngleRegex.Replace(detailMatch.Groups["program"].ToString(), String.Empty)))),
                                Department = department
                            });
                            results.Add(new UTCourse()
                        {
                            Code = detailMatch.Groups["code"].ToString(),
                            SemesterPrefix = detailMatch.Groups["prefix"].ToString(),
                            Semester = "S",
                            Name = detailMatch.Groups["name"].ToString().Trim(' '),
                            Description = detailMatch.Groups["description"].ToString().Trim(' '),
                            Prerequisites = prerequisites,
                            Corequisites = corequisites,
                            Exclusions = exclusions,
                            DistributionRequirement = distribution,
                            Program = String.Join(" ", GetMatchedValues(ProgramRegex.Matches(AngleRegex.Replace(detailMatch.Groups["program"].ToString(), String.Empty)))),
                            Department = department
                        });
                        }
                    }
                    else
                    {
                        lock (this)
                        {
                            results.Add(new UTCourse()
                            {
                                Code = detailMatch.Groups["code"].ToString(),
                                SemesterPrefix = detailMatch.Groups["prefix"].ToString(),
                                Semester = semester,
                                Name = detailMatch.Groups["name"].ToString().Trim(' '),
                                Description = detailMatch.Groups["description"].ToString().Trim(' '),
                                Prerequisites = prerequisites,
                                Corequisites = corequisites,
                                Exclusions = exclusions,
                                DistributionRequirement = distribution,
                                Program = String.Join(" ", GetMatchedValues(ProgramRegex.Matches(AngleRegex.Replace(detailMatch.Groups["program"].ToString(), String.Empty)))),
                                Department = department
                            });
                        }
                    }
                }
            });

            return results;
        }

        private IEnumerable<string> GetMatchedValues(MatchCollection collection)
        {
            return collection.Cast<Match>().Select<Match, string>(x => x.Value);
        }
    }
}
