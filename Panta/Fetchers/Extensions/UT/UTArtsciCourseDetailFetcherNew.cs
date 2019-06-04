using Panta.DataModels.Extensions.UT;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Panta.Fetchers.Extensions.UT
{
    public class UTArtsciCourseDetailFetcherNew : WebpageItemFetcher<UTCourse>
    {
        public UTArtsciCourseDetailFetcherNew(string url) : base(url) { }

        private static Regex DetailRegex;
        private static Regex AngleRegex;
        private static Regex TdRegex;
        private static Regex AsciiRegex;
        static UTArtsciCourseDetailFetcherNew()
        {
            DetailRegex = new Regex("<tr .*?class=[^>]+>(?<course>.*?)</tr>", RegexOptions.Multiline | RegexOptions.Compiled);
            AngleRegex = new Regex("<[^>]+>", RegexOptions.Compiled);
            TdRegex = new Regex("</td>", RegexOptions.Compiled);
            AsciiRegex = new Regex("[^\u0000-\u007F] + ", RegexOptions.Compiled);
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
                string text = match.Groups["course"].ToString();
                text = TdRegex.Replace(text, "|");
                text = AngleRegex.Replace(text, String.Empty);
                string[] props = text.Split('|');
                string[] properties = new string[props.Length];
                for (int j = 0; j < props.Length; j++)
                {
                    properties[j] = AsciiRegex.Replace(props[j].Trim(' '), String.Empty);
                }
                UTCourse courseObj = new UTCourse()
                {
                    Name = properties[1],
                    Code = properties[0].Substring(0, 6),
                    SemesterPrefix = properties[0].Substring(6),
                    Description = properties[2],
                    Prerequisites = properties[3],
                    Corequisites = String.Empty,
                    Exclusions = properties[4],
                    DistributionRequirement = String.Empty,
                    BreadthRequirement = String.Empty
                };
                results.Add(courseObj);
                Console.Out.WriteLine("CourseDetail: " + courseObj.Abbr);
            }
            return results;
        }
    }
}
