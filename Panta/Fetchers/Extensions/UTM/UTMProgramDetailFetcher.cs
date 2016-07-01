using Panta.DataModels;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace Panta.Fetchers.Extensions.UTM
{
    public class UTMProgramDetailFetcher : WebpageItemFetcher<SchoolProgram>
    {
        public UTMProgramDetailFetcher(string url) : base(url) { }

        private static Regex BrRegex, AngleRegex, ProgramRegex, AllProgramNoteRegex;

        static UTMProgramDetailFetcher()
        {
            BrRegex = new Regex("<br[^>]*>", RegexOptions.Compiled);
            AngleRegex = new Regex("<[^>]*>", RegexOptions.Multiline | RegexOptions.Compiled);
            ProgramRegex = new Regex("<p class=\"title_program\">(?<name>.*?)</p>(?<content>.*?)(?=(<p class=\"title_program)|(<dl>)|(</div>)|($))", RegexOptions.Multiline | RegexOptions.Compiled);
            AllProgramNoteRegex = new Regex("<b>NOTES FOR ALL PROGRAMS</b>.*?</dl>", RegexOptions.Compiled | RegexOptions.Multiline);
        }

        public override IEnumerable<SchoolProgram> FetchItems()
        {
            List<SchoolProgram> results = new List<SchoolProgram>();
            if (this.Content == null) return results;
            TextInfo formatter = CultureInfo.CurrentCulture.TextInfo;

            this.Content = this.Content.Replace("\n", String.Empty).Replace("\r", String.Empty);
            MatchCollection matches = ProgramRegex.Matches(this.Content);
            string note = AllProgramNoteRegex.Match(this.Content).Value;
            note = AngleRegex.Replace(BrRegex.Replace(HttpUtility.HtmlDecode(note), "|").Replace("</tr>", "|").Replace("</p>", "|").Replace("</div>", "|"), String.Empty);

            foreach (Match match in matches)
            {
                string content = HttpUtility.HtmlDecode(match.Groups["content"].Value);
                content = AngleRegex.Replace(BrRegex.Replace(content, "|").Replace("</tr>", "|").Replace("</p>", "|").Replace("</div>", "|"), String.Empty);

                SchoolProgram program = new SchoolProgram()
                {
                    Name = formatter.ToTitleCase(AngleRegex.Replace(HttpUtility.HtmlDecode(match.Groups["name"].Value), " ")).Trim(' '),
                    Description = (content + "|" + note).TrimEnd('|'),
                    Campus = "UTM"
                };

                Console.WriteLine("UTM Program: " + program.Name);
                results.Add(program);
            }

            return results;
        }
    }
}
