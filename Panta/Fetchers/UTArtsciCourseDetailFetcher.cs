using Panta.DataModels.Extensions.UT;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Text.RegularExpressions;

namespace Panta.Fetchers
{
    [Serializable]
    public class UTArtsciCourseDetailFetcher : IItemFetcher<UTCourse>
    {
        public string Url { get; set; }

        public UTArtsciCourseDetailFetcher(string url)
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

            content = content.Replace("\r\n", String.Empty);

            Regex detailRegex = new Regex("<a name=\"(?<abbr>[A-Z]{3}[0-9]{3})(?<prefix>[HY][10])\".*?<p>" +
                "(?<description>[^<]*)</p>" +
                "(?:(?:[^P]*)(?:Prerequisite:)[^<]*(?:<a href=[^>]+>(?<prerequisite>[A-Z0-9]{8})</a>[^<]*)+)?" +
                "(?:(?:[^E]*)(?:Exclusion:)(?:[^<]*)(?:(?:<a href=[^>]+>)?(?:[^A-Z]*)(?<exclusion>[A-Z0-9]{8})(?:</a>)?[^<]*)+)?" +
                "(?:(?:[^D]*)(?<distribution>Distribution[^<]*))?" +
                "(?:(?:[^B]*)(?<breadth>Breadth[^<]*))?",
                RegexOptions.Multiline | RegexOptions.Compiled);

            MatchCollection matches = detailRegex.Matches(content);

            foreach (Match match in matches)
            {
                string abbr = match.Groups["abbr"].ToString();
                string prefix = match.Groups["prefix"].ToString();
                string description = match.Groups["description"].ToString();
                string distribution = match.Groups["distribution"].ToString().TrimStart(' ').TrimEnd(' ');
                string breadth = match.Groups["breadth"].ToString().TrimStart(' ').TrimEnd(' ');
                List<string> prerequisites = new List<string>();
                List<string> exclusions = new List<string>();
                foreach (Capture capture in match.Groups["prerequisite"].Captures)
                {
                    prerequisites.Add(capture.ToString());
                }
                foreach (Capture capture in match.Groups["exclusion"].Captures)
                {
                    exclusions.Add(capture.ToString());
                }
                results.Add(new UTCourse()
                {
                    Abbr = abbr,
                    SemesterPrefix = prefix,
                    Description = description,
                    Prerequisites = prerequisites,
                    Exclusions = exclusions,
                    DistributionRequirement = distribution,
                    BreadthRequirement = breadth
                });
            }
            return results;
        }
    }
}
