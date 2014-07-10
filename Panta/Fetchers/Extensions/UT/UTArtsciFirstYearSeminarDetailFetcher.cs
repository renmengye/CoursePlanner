using Panta.DataModels.Extensions.UT;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace Panta.Fetchers.Extensions.UT
{
    public class UTArtsciFirstYearSeminarDetailFetcher : WebpageItemFetcher<UTCourse>
    {
        public UTArtsciFirstYearSeminarDetailFetcher(string url) : base(url) { }

        private static Regex DetailRegex, CodeRegex, AngleRegex, BrRegex;

        static UTArtsciFirstYearSeminarDetailFetcher()
        {
            DetailRegex = new Regex("<a name=\"(?<section>[L][0-9]{4}).*?(?=(<hr />)|($))", RegexOptions.Compiled | RegexOptions.Multiline);
            CodeRegex = new Regex("[A-Z]{3} [0-9]{3}[HY][0-9][FSY]", RegexOptions.Compiled);
            AngleRegex = new Regex("<[^>]+>", RegexOptions.Compiled);
            BrRegex = new Regex("<br[^>]*>", RegexOptions.Compiled);
        }

        public override IEnumerable<UTCourse> FetchItems()
        {
            List<UTCourse> results = new List<UTCourse>();

            this.Content = this.Content.Replace("\n", String.Empty).Replace("\r", String.Empty);
            MatchCollection matches = DetailRegex.Matches(this.Content);
            foreach (Match match in matches)
            {
                string[] properties = AngleRegex.Replace(BrRegex.Replace(match.Value.Replace("</p>", "|"), "|"), String.Empty).Split('|');
                Match codeMatch = CodeRegex.Match(match.Value);

                UTCourse course = new UTCourse()
                {
                    Code = codeMatch.Value.Replace(" ", String.Empty),
                    SemesterPrefix = codeMatch.Groups["prefix"].Value,
                    Semester = codeMatch.Groups["semester"].Value,
                    Description = "",
                    Corequisites = match.Groups["section"].Value
                };

                foreach (string property in properties)
                {
                    if (property.Length > course.Description.Length)
                    {
                        course.Description = HttpUtility.HtmlDecode(property.Trim(' '));
                    }
                    if (property.Trim(' ').StartsWith("Breadth"))
                    {
                        course.BreadthRequirement = property.Trim(' ').Substring("Breadth category: ".Length);
                    }
                }
                results.Add(course);
            }
            return results;
        }
    }
}
