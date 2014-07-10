using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Panta.Fetchers.Extensions.UT
{
    public class UTEngHssFetcher : WebpageItemFetcher<string>
    {
        public UTEngHssFetcher(string url) : base(url) { }

        private static Regex TableRegex, CodeRegex;

        static UTEngHssFetcher()
        {
            TableRegex = new Regex("<table.*?</table>", RegexOptions.Compiled | RegexOptions.Multiline);
            CodeRegex = new Regex("[A-Z]{3}[0-9]{3}[HY][0-9]", RegexOptions.Compiled);
        }

        public override IEnumerable<string> FetchItems()
        {
            if (this.Content == null) return new List<string>();
            string tableContent = this.Content.Replace("\n", String.Empty);
            tableContent = tableContent.Replace("\r", String.Empty);
            tableContent = TableRegex.Match(tableContent).Value;
            return CodeRegex.Matches(tableContent).Cast<Match>().Select(x => x.Value);
        }
    }
}
