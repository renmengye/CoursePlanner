using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Panta.Fetchers.Extensions.UTM
{
    public class UTMProgramUrlFetcher : WebpageItemFetcher<string>
    {
        public UTMProgramUrlFetcher(string url) : base(url) { }

        public const string Root = "https://registrar.utm.utoronto.ca/student/calendar/";

        private static Regex ProgramUrlRegex;

        static UTMProgramUrlFetcher()
        {
            ProgramUrlRegex = new Regex("program_group[\u002e]pl[\u003f]Group_Id=[0-9]*", RegexOptions.Multiline | RegexOptions.Compiled);
        }

        public override IEnumerable<string> FetchItems()
        {
            List<string> results = new List<string>();

            this.Content = this.Content.Replace("\n", String.Empty).Replace("\r", String.Empty);
            foreach (Match match in ProgramUrlRegex.Matches(this.Content))
            {
                results.Add(Root + match.Value);
            }

            return results;
        }
    }
}
