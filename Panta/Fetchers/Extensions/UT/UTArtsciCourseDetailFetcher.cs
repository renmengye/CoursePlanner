using Panta.DataModels.Extensions.UT;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Text.RegularExpressions;

namespace Panta.Fetchers.Extensions.UT
{
    [Serializable]
    public class UTArtsciCourseDetailFetcher : WebpageItemFetcher<UTCourse>
    {
        public UTArtsciCourseDetailFetcher(string url) : base(url) { }

        private static Regex DetailRegex, AngleRegex, CodeRegex;

        static UTArtsciCourseDetailFetcher()
        {
            DetailRegex = new Regex("<a name=\"(?<code>[A-Z]{3}[0-9]{3})(?<prefix>[HY][10])\".*?<p>" +
                "(?<description>.*?)</p>(?<more>.*?)(?=(<a name=)|(<div id=\"footer))", RegexOptions.Compiled);
            AngleRegex = new Regex("<[^>]+>", RegexOptions.Compiled);
            CodeRegex = new Regex("(?<code>[A-Z]{3}[0-9]{3})(?<prefix>[HY][1])", RegexOptions.Compiled);
        }

        public override IEnumerable<UTCourse> FetchItems()
        {
            List<UTCourse> results = new List<UTCourse>();

            if (this.Content == null) return results;
            this.Content = this.Content.Replace("\r", String.Empty);
            this.Content = this.Content.Replace("\n", String.Empty);

            MatchCollection matches = DetailRegex.Matches(this.Content);
            foreach (Match match in matches)
            {
                string text = match.Groups["more"].ToString();

                // Match preq, creq etc.
                text = text.Replace("<br>", "|");
                text = AngleRegex.Replace(text, String.Empty);

                string prerequisites = null;
                string corequisites = null;
                string exclusions = null;
                string distribution = null;
                string breadth = null;

                string[] properties = text.Split('|');
                for (int i = 0; i < properties.Length; i++)
                {
                    if (properties[i].StartsWith("Prerequisite:"))
                    {
                        prerequisites = properties[i].Substring("Prerequisite:".Length).Trim(' ').Replace("/", " / ");
                    }
                    else if (properties[i].StartsWith("Corequisite:"))
                    {
                        corequisites = properties[i].Substring("Corequisite:".Length).Trim(' ').Replace("/", " / ");
                    }
                    else if (properties[i].StartsWith("Exclusion:"))
                    {
                        exclusions = properties[i].Substring("Exclusion:".Length).Trim(' ').Replace("/", " / ");
                    }
                    else if (properties[i].StartsWith("Distribution Requirement Status:"))
                    {
                        distribution = properties[i].Substring("Distribution Requirement Status:".Length).Trim(' ');
                    }
                    else if (properties[i].StartsWith("Breadth Requirement:"))
                    {
                        breadth = properties[i].Substring("Breadth Requirement:".Length).Trim(' ');
                    }
                }

                results.Add(new UTCourse()
                {
                    Code = match.Groups["code"].ToString(),
                    SemesterPrefix = match.Groups["prefix"].ToString(),
                    Description = AngleRegex.Replace(match.Groups["description"].ToString(), String.Empty),
                    Prerequisites = prerequisites,
                    Corequisites = corequisites,
                    Exclusions = exclusions,
                    DistributionRequirement = distribution,
                    BreadthRequirement = breadth
                });
            }

            return results;
        }
    }
}
