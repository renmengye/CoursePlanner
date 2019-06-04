using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Panta.Fetchers.Extensions.UT
{
    // Returns a list of total page numbers (only expect 1 page number).
    public class UTArtsciCourseDetailPageNumberFetcher : WebpageItemFetcher<int>
    {
        public UTArtsciCourseDetailPageNumberFetcher(string url) : base(url) { }

        private static Regex PageNumberRegex;

        static UTArtsciCourseDetailPageNumberFetcher()
        {
            PageNumberRegex = new Regex("<li class=\"pager-current\">1 of (?<number>[0-9]+)</li>", RegexOptions.Compiled);
        }
        public override IEnumerable<int> FetchItems()
        {
            List<int> results = new List<int>();

            if (this.Content == null) return results;
            this.Content = this.Content.Replace("\r", String.Empty);
            this.Content = this.Content.Replace("\n", String.Empty);
            MatchCollection matches = PageNumberRegex.Matches(this.Content);
            foreach (Match match in matches)
            {
                int pagenumber = 0;
                Int32.TryParse(match.Groups["number"].ToString(), out pagenumber);
                results.Add(pagenumber);
            }
            return results;
        }
    }
}
