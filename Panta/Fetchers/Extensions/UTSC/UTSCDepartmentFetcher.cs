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
        public UTSCDepartmentFetcher(string url, string sess="year")
            : base(url, string.Format("sess={0:S}&submit=&course=", sess))
        {
            this.DetailUrls = new HashSet<string>();
        }

        static UTSCDepartmentFetcher()
        {
            TableRegex = new Regex("<div id=\"main-content\".*?</div>", RegexOptions.Multiline | RegexOptions.Compiled);
            LinkRegex = new Regex("<a href=\"(?<link>[^\"]*)\"[^<]*</a>", RegexOptions.Compiled);
            FullNameRegex = new Regex("full_name=(?<name>.*)", RegexOptions.Compiled);
        }

        public HashSet<string> DetailUrls { get; set; }

        private static Regex TableRegex, LinkRegex, FullNameRegex;

        public override IEnumerable<string> FetchItems()
        {
            this.Content = this.Content.Replace("\n", "").Replace("\r", "");
            string tableContent = TableRegex.Match(this.Content).Value;
            MatchCollection matches = LinkRegex.Matches(tableContent);

            foreach (Match match in matches)
            {
                string link = match.Groups["link"].Value;
                string[] urlParts = this.Url.Split('/');
                string fullName = FullNameRegex.Match(link).Groups["name"].Value;
                fullName = fullName.Replace(" ", "_");
                //urlParts[urlParts.Length - 1] = link;
                urlParts[urlParts.Length - 1] = fullName + ".html";
                //urlParts = (string[])urlParts.Take(urlParts.Length - 1);
                link = string.Join("/", urlParts);
                Console.Out.WriteLine(link);
                if (!this.DetailUrls.Contains(link))
                {
                    this.DetailUrls.Add(link);
                }
            }
            return this.DetailUrls;
        }
    }
}
