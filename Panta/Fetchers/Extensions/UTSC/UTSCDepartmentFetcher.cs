using Panta.DataModels.Extensions.UT;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Panta.Fetchers.Extensions.UTSC
{
    public class UTSCDepartmentFetcher : WebpageItemFetcher<string>
    {
        public UTSCDepartmentFetcher(string url)
            : base(url, "sess=year&submit=&course=")
        {
            this.DetailUrls = new HashSet<string>();
        }

        static UTSCDepartmentFetcher()
        {
            TableRegex = new Regex("<div id=\"timetable_section\".*?</div>", RegexOptions.Multiline | RegexOptions.Compiled);
            LinkRegex = new Regex("<a href=\"(?<link>http[^#]*)[^<]*</a>", RegexOptions.Compiled);
        }

        public HashSet<string> DetailUrls { get; set; }

        private static Regex TableRegex, LinkRegex;

        public override IEnumerable<string> FetchItems()
        {
            this.Content = this.Content.Replace("\n", "").Replace("\r", "");
            this.Content = TableRegex.Match(this.Content).Value;
            MatchCollection matches = LinkRegex.Matches(this.Content);

            foreach (Match match in matches)
            {
                string link = match.Groups["link"].Value;
                if (!this.DetailUrls.Contains(link))
                {
                    this.DetailUrls.Add(link);
                }
            }
            return this.DetailUrls;
        }
    }
}
