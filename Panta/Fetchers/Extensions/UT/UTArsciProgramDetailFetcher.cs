using Panta.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace Panta.Fetchers.Extensions.UT
{
    public class UTArtsciProgramDetailFetcher : WebpageItemFetcher<SchoolProgram>
    {
        public UTArtsciProgramDetailFetcher(string url) : base(url) { }

        private static Regex DetailRegex, AngleRegex, CodeRegex, StartRegex;
        //BrRegex, EcoRegex;

        static UTArtsciProgramDetailFetcher()
        {
            StartRegex = new Regex("<span class=\"heading2\">.*?Programs</span>(?<content>.*?)(?=<h2>)");
            DetailRegex = new Regex("<span class=\"strong\">(?<name>[^<]*)</span>(?<content>.*?)(?=(<span class=\"strong)|(<a name=))");
            AngleRegex = new Regex("<[^>]+>", RegexOptions.Compiled);
            CodeRegex = new Regex("(?<code>[A-Z]{3}[0-9]{3})(?<prefix>[HY][1])", RegexOptions.Compiled);
        }

        public override IEnumerable<SchoolProgram> FetchItems()
        {
            List<SchoolProgram> results = new List<SchoolProgram>();
            if (this.Content != null)
            {
                this.Content = this.Content.Replace("\r", String.Empty);
                this.Content = this.Content.Replace("\n", String.Empty);
                Match contentMatch = StartRegex.Match(this.Content);
                if (contentMatch.Success)
                {
                    MatchCollection matches = DetailRegex.Matches(contentMatch.Groups["content"].ToString());
                    foreach (Match match in matches)
                    {
                        string name = match.Groups["name"].ToString();
                        string content = match.Groups["content"].ToString();

                        name = HttpUtility.HtmlDecode(name);
                        content = HttpUtility.HtmlDecode(content);
                        content = AngleRegex.Replace(content, String.Empty);

                        SchoolProgram program = new SchoolProgram()
                        {
                            Name = name,
                            Description = content
                        };
                        results.Add(program);
                    }
                }
            }
            return results;
        }
    }
}
