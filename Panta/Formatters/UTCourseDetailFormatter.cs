using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Text.RegularExpressions;
using Panta.DataModels;

namespace Panta.Formatters
{
    [Serializable]
    public class UTCourseDetailFormatter : IWebpageFormatter<Course>
    {
        public string Url { get; set; }

        public UTCourseDetailFormatter(string url)
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

            //content = "<a name=\"ABS351Y1\"></a><span class=\"strong\">ABS351Y1&nbsp;&nbsp;&nbsp;&nbsp;Indigenous Legends &amp; Teaching (formerly ABS351H1)[24L]</span><p>An introduction to laws of Indigenous societies, focusing on the Anishinaabe, as seen through legends and teachings.</p>\r\n<p>Prerequisite:\r\n                    <a href=\"crs_abs.htm#ABS201Y1\">ABS201Y1</a><br>Exclusion:\r\nABS351H1<br>Distribution Requirement Status:  This is a Humanities or Social Science course<br>Breadth Requirement:  Creative and Cultural Representations (1)</p>\r\n<p><strong>";

            content = content.Replace("\r\n", String.Empty);

            Regex detailRegex = new Regex("<a name=\"(?<code>[A-Z0-9]{8})\".*?<p>" +
                "(?<description>[^<]*)</p>" +
                "(?:(?:[^P]*)(?:Prerequisite:)[^<]*(?:<a href=[^>]+>(?<prerequisite>[A-Z0-9]{8})</a>[^<]*)+)?" +
                "(?:(?:[^E]*)(?:Exclusion:)(?:[^<]*)(?:(?:<a href=[^>]+>)?(?:[^A-Z]*)(?<exclusion>[A-Z0-9]{8})(?:</a>)?[^<]*)+)?" +
                "(?:(?:[^D]*)(?<distribution>Distribution[^<]*))?" +
                "(?:(?:[^B]*)(?<breadth>Breadth[^<]*))?",
                RegexOptions.Multiline | RegexOptions.Compiled);

            MatchCollection matches = detailRegex.Matches(content);

            foreach (Match match in matches)
            {
                string code = match.Groups["code"].ToString();
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
                results.Add(new Course()
                {
                    Code = code,
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
