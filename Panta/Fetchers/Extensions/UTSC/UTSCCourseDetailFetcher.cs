using Panta.DataModels.Extensions.UT;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace Panta.Fetchers.Extensions.UTSC
{
    public class UTSCCourseDetailFetcher : WebpageItemFetcher<UTCourse>
    {
        public UTSCCourseDetailFetcher(string url) : base(url) { }

        private static Regex CourseRegex, AngleRegex, SpaceRegex, UrlRegex;

        static UTSCCourseDetailFetcher()
        {
            CourseRegex = new Regex("<a name=\"(?<code>[A-Z]{4}[0-9]{2})(?<prefix>[HY][0-9])\"></a><span.*?<a.*?</a>(?<name>[^<]*)(?<content>.*?)(?=(<a name)|(</td>))", RegexOptions.Compiled | RegexOptions.Multiline);
            AngleRegex = new Regex("<[^>]*>", RegexOptions.Multiline | RegexOptions.Compiled);
            SpaceRegex = new Regex("( ){2,}", RegexOptions.Compiled);
            UrlRegex = new Regex("(?<name>[^/]*).html", RegexOptions.Compiled);
        }

        public override IEnumerable<UTCourse> FetchItems()
        {
            List<UTCourse> results = new List<UTCourse>();
            if (this.Content == null) { return results; }
            this.Content = this.Content.Replace("\n", String.Empty).Replace("\r", String.Empty);
            MatchCollection matches = CourseRegex.Matches(this.Content);

            foreach (Match match in matches)
            {
                UTCourse course = new UTCourse()
                {
                    Code = match.Groups["code"].Value,
                    SemesterPrefix = match.Groups["prefix"].Value,
                    Name = SpaceRegex.Replace(HttpUtility.HtmlDecode(match.Groups["name"].Value).Trim(' ').Trim(' '), " ")
                };
                string[] properties = AngleRegex.Replace(match.Groups["content"].Value.Replace("<br>", "|"), String.Empty).Split('|');

                string description = String.Empty;
                bool afterDescription = false;
                foreach (string property in properties)
                {
                    if (property.Trim(' ').StartsWith("Prerequisite: "))
                    {
                        course.Prerequisites = SpaceRegex.Replace(HttpUtility.HtmlDecode(property.Trim(' ').Substring("Prerequisite: ".Length)), " ");
                        afterDescription = true;
                    }
                    else if (property.Trim(' ').StartsWith("Exclusion: "))
                    {
                        course.Exclusions = SpaceRegex.Replace(HttpUtility.HtmlDecode(property.Trim(' ').Substring("Exclusion: ".Length)), " ");
                        afterDescription = true;
                    }
                    else if (property.Trim(' ').StartsWith("Breadth Requirement: "))
                    {
                        course.Exclusions = SpaceRegex.Replace(HttpUtility.HtmlDecode(property.Trim(' ').Substring("Breadth Requirement: ".Length)), " ");
                        afterDescription = true;
                    }
                    else if (property.Trim(' ').StartsWith("Corequisite: "))
                    {
                        course.Corequisites = SpaceRegex.Replace(HttpUtility.HtmlDecode(property.Trim(' ').Substring("Corequisite: ".Length)), " ");
                        afterDescription = true;
                    }
                    else if (!afterDescription)
                    {
                        description += property;
                    }
                }
                course.Description = description;
                course.Department = SpaceRegex.Replace(HttpUtility.HtmlDecode(description), " ").Trim(' ');
                course.Department = UrlRegex.Match(this.Url).Groups["name"].Value.Replace("_", " ");
                results.Add(course);

                Console.WriteLine("UTSC Course: " + course.Abbr);
            }

            return results;
        }
    }
}
