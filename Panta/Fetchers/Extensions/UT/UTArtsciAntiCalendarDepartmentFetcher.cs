using Panta.DataModels.Extensions.UT;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace Panta.Fetchers.Extensions.UT
{
    public class UTArtsciAntiCalendarDepartmentFetcher : WebpageItemFetcher<UTDepartment>
    {
        public UTArtsciAntiCalendarDepartmentFetcher(string url) : base(url) { }
        private static Regex DepartmentRegex;

        static UTArtsciAntiCalendarDepartmentFetcher()
        {
            DepartmentRegex = new Regex("<a href=\"(?<url>http://assu.ca/wp/wp-content/uploads[^\"]*)[^>]*>(?<name>[^<]*)",
                RegexOptions.Multiline | RegexOptions.Compiled);
        }

        public override IEnumerable<UTDepartment> FetchItems()
        {
            List<UTDepartment> result = new List<UTDepartment>();
            if (this.Content == null) return result;

            MatchCollection matches = DepartmentRegex.Matches(this.Content);

            foreach (Match match in matches)
            {
                UTDepartment dep = new UTDepartment()
                {
                    Name = HttpUtility.HtmlDecode(match.Groups["name"].Value),
                    Url = match.Groups["url"].Value
                };
                result.Add(dep);
            }

            return result;
        }
    }
}
