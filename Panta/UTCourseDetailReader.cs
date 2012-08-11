using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Text.RegularExpressions;
using System.Diagnostics;

namespace Panta
{
    [Serializable]
    public class UTCourseDetailReader : IFormatReader<Course>
    {
        public string Url { get; set; }

        public UTCourseDetailReader(string url)
        {
            this.Url = url;
        }

        public IEnumerable<Course> Read()
        {
            List<Course> results = new List<Course>();
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


            Regex detailRegex = new Regex("<a name=\"(?<code>[A-Z0-9]{8})\".*?<p>" +
                "(?<description>[^<]*)</p>" +
                "(?:(?:Prerequisite:)[^<]*(?:<a href=[^>]*>(?<prerequisite>[A-Z0-9]{8})</a>[^<]*)+)?(?:)?" +
                "(?:.*?Distribution Requirement Status: (?<distribution>[^<]*))?" +
                "(?:.*?Breadth Requirement: (?<breadth>[^<]*))?",
                RegexOptions.Multiline | RegexOptions.Compiled | RegexOptions.IgnoreCase);

            MatchCollection matches = detailRegex.Matches(content);

            foreach (Match match in matches)
            {
                string code = match.Groups["code"].ToString();
                string description = match.Groups["description"].ToString();
                string distribution = match.Groups["distribution"].ToString().TrimStart(' ').TrimEnd(' ');
                string breadth = match.Groups["breadth"].ToString().TrimStart(' ').TrimEnd(' ');
                List<string> prerequisites = new List<string>();
                foreach (Capture capture in match.Groups["prerequisite"].Captures)
                {
                    prerequisites.Add(capture.ToString());
                }
                results.Add(new Course()
                {
                    Code = code,
                    Description = description,
                    Prerequisites = prerequisites,
                    DistributionRequirement = distribution,
                    BreadthRequirement = breadth
                });
            }
            return results;
        }
    }
}
