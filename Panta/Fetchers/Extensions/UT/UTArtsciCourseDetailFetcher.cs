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

        private static Regex DetailRegex;

        static UTArtsciCourseDetailFetcher()
        {
            DetailRegex = new Regex("<a name=\"(?<code>[A-Z]{3}[0-9]{3})(?<prefix>[HY][10])\".*?<p>" +
                "(?<description>[^<]*)</p>" +
                "(?:(?:[^P]*)(?:Prerequisite:)[^<]*(?:<a href=[^>]+>(?<prerequisite>[A-Z0-9]{8})</a>[^<]*)+)?" +
                "(?:(?:[^C]*)(?:Corequisite:)[^<]*(?:<a href=[^>]+>(?<corequisite>[A-Z0-9]{8})</a>[^<]*)+)?" +
                "(?:(?:[^E]*)(?:Exclusion:)(?:[^<]*)(?:(?:<a href=[^>]+>)?(?:[^A-Z]*)(?<exclusion>[A-Z0-9]{8})(?:</a>)?[^<]*)+)?" +
                "(?:(?:[^D]*)(?<distribution>Distribution[^<]*))?" +
                "(?:(?:[^B]*)(?<breadth>Breadth[^<]*))?", RegexOptions.Compiled);
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
                results.Add(new UTCourse()
                {
                    Code = match.Groups["code"].ToString(),
                    SemesterPrefix = match.Groups["prefix"].ToString(),
                    Description = match.Groups["description"].ToString(),
                    Prerequisites = String.Join(" ", match.Groups["prerequisite"].Captures.Cast<Capture>().Select<Capture, string>(capture => capture.ToString())),
                    Corequisites = String.Join(" ", match.Groups["corequisite"].Captures.Cast<Capture>().Select<Capture, string>(capture => capture.ToString())),
                    Exclusions = String.Join(" ", match.Groups["exclusion"].Captures.Cast<Capture>().Select<Capture, string>(capture => capture.ToString())),
                    DistributionRequirement = match.Groups["distribution"].ToString().TrimStart(' ').TrimEnd(' '),
                    BreadthRequirement = match.Groups["breadth"].ToString().TrimStart(' ').TrimEnd(' ')
                });
            }

            return results;
        }
    }
}
