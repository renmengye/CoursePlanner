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
        private static Regex SpanRegex;
        private static Regex PRegex;
        private static Regex AsciiRegex;
        static UTArtsciCourseDetailFetcherNew()
        {
            DetailRegex = new Regex("<div class=\"views-row\">.*?<div class=\"views-row\">.*?</div>[\\s]*</div>.*?</div>[\\s]*</div>", RegexOptions.Multiline | RegexOptions.Compiled);
            AngleRegex = new Regex("<[^>]+>", RegexOptions.Compiled);
            SpanRegex = new Regex("</span>", RegexOptions.Compiled);
            PRegex = new Regex("</p>", RegexOptions.Compiled);
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
                string text = match.Value;
                //string text = match.Groups["course"].ToString();
                text = SpanRegex.Replace(text, "|");
                text = PRegex.Replace(text, "|");
                text = AngleRegex.Replace(text, String.Empty);
                string[] props = text.Split('|');
                string[] properties = new string[props.Length];
                Dictionary<string, string> propdict = new Dictionary<string, string>();
                string description = String.Empty;
                for (int j = 0; j < props.Length; j++)
                {
                    properties[j] = AsciiRegex.Replace(props[j].Trim(' '), String.Empty);
                    if (properties[j].Length > 0 && j > 0 && description.Length == 0 && !properties[j].StartsWith("Hours")) {
                        description = properties[j];
                    }
                    else if (properties[j].StartsWith("Exclusion"))
                    {
                        propdict["Exclusion"] = properties[j].Split(':')[1].Trim(' ');
                    }
                    else if (properties[j].StartsWith("Breadth"))
                    {
                        propdict["Breadth"] = properties[j].Split(':')[1].Trim(' ');
                    }
                    else if (properties[j].StartsWith("Distribution"))
                    {
                        propdict["Distribution"] = properties[j].Split(':')[1].Trim(' ');
                    }
                    else if (properties[j].StartsWith("Prerequisite"))
                    {
                        propdict["Prerequisite"] = properties[j].Split(':')[1].Trim(' ');
                    }
                    else if (properties[j].StartsWith("Corequisite"))
                    {
                        propdict["Corequisite"] = properties[j].Split(':')[1].Trim(' ');
                    }
                }
                UTCourse courseObj = new UTCourse()
                {
                    Name = properties[0].Substring(11),
                    Code = properties[0].Substring(0, 6),
                    SemesterPrefix = properties[0].Substring(6, 2),
                    Description = description,
                    Prerequisites = propdict.ContainsKey("Prerequisite") ? propdict["Prerequisite"] : String.Empty,
                    Corequisites = propdict.ContainsKey("Corequisite") ? propdict["Corequisite"] : String.Empty,
                    Exclusions = propdict.ContainsKey("Exclusion") ? propdict["Exclusion"] : String.Empty,
                    DistributionRequirement = propdict.ContainsKey("Distribution") ? propdict["Distribution"] : String.Empty,
                    BreadthRequirement = propdict.ContainsKey("Breadth") ? propdict["Breadth"] : String.Empty
                };
                results.Add(courseObj);
                Console.Out.WriteLine("CourseDetail: " + courseObj.Abbr);
            }
            return results;
        }
    }
}
