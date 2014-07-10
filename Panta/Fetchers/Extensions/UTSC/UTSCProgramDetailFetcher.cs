using Panta.DataModels;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Panta.Fetchers.Extensions.UTSC
{
    public class UTSCProgramDetailFetcher : WebpageItemFetcher<SchoolProgram>
    {
        public UTSCProgramDetailFetcher(string url) : base(url) { }

        private static Regex AngleRegex, SpaceRegex, H2Regex, ProgramRegex;

        static UTSCProgramDetailFetcher()
        {
            AngleRegex = new Regex("<[^>]*>", RegexOptions.Multiline | RegexOptions.Compiled);
            SpaceRegex = new Regex("( ){2,}", RegexOptions.Compiled);
            H2Regex = new Regex("<h2>[^<]*?Programs</h2>.*?<h2>", RegexOptions.Compiled | RegexOptions.Multiline);
            ProgramRegex = new Regex("<a name=[^>]*></a><span[^>]*>(?<name>[^<]*)</span>(?<content>.*?)(?=(<a name=)|(<h2))", RegexOptions.Multiline | RegexOptions.Compiled);
        }


        public override IEnumerable<SchoolProgram> FetchItems()
        {
            List<SchoolProgram> results = new List<SchoolProgram>();
            TextInfo formatter = CultureInfo.CurrentCulture.TextInfo;
            if (this.Content == null) { return results; }
            this.Content = this.Content.Replace("\n", String.Empty).Replace("\r", String.Empty);
            string programInfo = H2Regex.Match(this.Content).Value;
            MatchCollection matches = ProgramRegex.Matches(programInfo);

            foreach (Match match in matches)
            {
                SchoolProgram program = new SchoolProgram()
                {
                    Name = formatter.ToTitleCase(SpaceRegex.Replace(match.Groups["name"].Value, " ").ToLowerInvariant()).Trim(' '),
                    Description = SpaceRegex.Replace(AngleRegex.Replace(match.Groups["content"].Value.Replace("<br>", "|").Replace("<p>", "|"), String.Empty), " ").Trim('\n').Trim(' ').Trim('\n'),
                    Campus = "UTSC"
                };
                Console.WriteLine("UTSC Program: " + program.Name);
                results.Add(program);
            }

            return results;
        }
    }
}
