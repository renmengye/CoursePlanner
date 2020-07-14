using Panta.DataModels.Extensions.UT;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Panta.Fetchers.Extensions.UT
{
    // Using CourseFinder to fetch one course at a time. Maybe slow.
    public class UTCourseDetailFetcher : WebpageItemFetcher<UTCourse>
    {
        public UTCourseDetailFetcher(string url) : base(url) { }

        private static Regex KeyValueRegex;
        private static Regex TitleRegex;

        static UTCourseDetailFetcher()
        {
            TitleRegex = new Regex("<span class=\"uif-headerText-span\">(?<code>[A-Z]{3}[0-9]{3})(?<prefix>[HY][135]): (?<name>[^<]*)</span>", RegexOptions.Compiled);
            KeyValueRegex = new Regex("<label[^<]+>(?<key>[^<]*)</label></span><span[^<]+>(?<value>[^<]*)</span>", RegexOptions.Compiled);
        }

        public override IEnumerable<UTCourse> FetchItems()
        {
            List<UTCourse> results = new List<UTCourse>();
            if (this.Content == null) return results;

            this.Content = this.Content.Replace("\r", String.Empty);
            this.Content = this.Content.Replace("\n", String.Empty);
            Match titleMatch = TitleRegex.Match(this.Content);
            MatchCollection matches = KeyValueRegex.Matches(this.Content);
            List<string> keys = new List<string>();
            List<string> values = new List<string>();
            Dictionary<string, string> properties = new Dictionary<string, string>();
            foreach (Match match in matches)
            {
                string key = match.Groups["key"].ToString().Trim(' ');
                string value = match.Groups["value"].ToString().Trim(' ');
                properties.Add(key, value);
            }
            results.Add(new UTCourse()
            {
                Code = titleMatch.Groups["code"].ToString(),
                SemesterPrefix = titleMatch.Groups["prefix"].ToString(),
                Name = titleMatch.Groups["name"].ToString().Trim(' ').Replace("&amp;", "&"),
                Description = properties.ContainsKey("Course Description") ? properties["Course Description"] : null,
                Prerequisites = properties.ContainsKey("Pre-requisites") ? properties["Pre-requisites"] : null,
                Corequisites = properties.ContainsKey("Corequisite") ? properties["Corequisite"] : null,
                Exclusions = properties.ContainsKey("Exclusion") ? properties["Exclusion"] : null,
                DistributionRequirement = null,
                Program = null,
                Department = properties.ContainsKey("Department") ? properties["Department"].Replace(" & amp;", "&") : null
            });
            return results;
        }
    }

}
