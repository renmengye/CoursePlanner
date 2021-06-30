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
            PageNumberRegex = new Regex("page=(?<number>[0-9]+)", RegexOptions.Compiled);
        }
        public override IEnumerable<int> FetchItems()
        {
            //List<int> results = new List<int>();

            if (this.Content == null) return new List<int>();
            this.Content = this.Content.Replace("\r", String.Empty);
            this.Content = this.Content.Replace("\n", String.Empty);
            MatchCollection matches = PageNumberRegex.Matches(this.Content);
            int max = -1;
            int min = Int32.MaxValue;
            foreach (Match match in matches)
            {
                int pagenumber = 0;
                Int32.TryParse(match.Groups["number"].ToString(), out pagenumber);
                if (pagenumber > max)
                {
                    max = pagenumber;
                }
                if (pagenumber < min)
                {
                    min = pagenumber;
                }
                //results.Add(pagenumber);
            }
            var results = Enumerable.Range(min, max - min + 1);
            return results;
        }
    }
}
