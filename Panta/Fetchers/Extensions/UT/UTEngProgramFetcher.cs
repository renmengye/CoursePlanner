using Panta.DataModels;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace Panta.Fetchers.Extensions.UT
{
    public class UTEngProgramFetcher : WebpageItemFetcher<SchoolProgram>
    {
        public UTEngProgramFetcher(string url) : base(url) { }

        private static Regex ProgramRegex, AngleRegex, BrRegex;

        static UTEngProgramFetcher()
        {
            ProgramRegex = new Regex("<a name=\"[A-Z]{5,}\"></a><span class=\"strong\">(?<name>[^<]*)</span>(?<content>.+?)(?=(<a name=\"AE[A-Z]{3,})|(<div id=\"footer\">))", RegexOptions.Compiled);
            AngleRegex = new Regex("<[^>]+>", RegexOptions.Compiled);
            BrRegex = new Regex("<br[^>]*>", RegexOptions.Compiled);
        }


        public override IEnumerable<SchoolProgram> FetchItems()
        {
            List<SchoolProgram> results = new List<SchoolProgram>();
            TextInfo formatter = CultureInfo.CurrentCulture.TextInfo;
            if (this.Content != null)
            {
                this.Content = this.Content.Replace("\r", String.Empty);
                this.Content = this.Content.Replace("\n", String.Empty);

                MatchCollection programMatches = ProgramRegex.Matches(this.Content);

                foreach (Match programMatch in programMatches)
                {
                    string name = programMatch.Groups["name"].Value;
                    string content = programMatch.Groups["content"].Value;

                    name = HttpUtility.HtmlDecode(name);
                    content = HttpUtility.HtmlDecode(content);

                    content = BrRegex.Replace(content, "|");
                    content = content.Replace("</tr>", "|");
                    content = content.Replace("</p>", "|");
                    content = content.Replace("</div>", "|");
                    content = content.Replace("</td>", " ");
                    content = AngleRegex.Replace(content, String.Empty);

                    SchoolProgram program = new SchoolProgram()
                    {
                        Name = formatter.ToTitleCase(name.ToLowerInvariant()).Trim(' '),
                        Description = content,
                        Campus = "UTSG"
                    };

                    Console.WriteLine("Engineering Program: " + name);

                    results.Add(program);
                }
            }
            return results;
        }
    }
}
