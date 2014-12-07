using Panta.DataModels.Extensions.UT;
using Panta.Fetchers.Extensions.UT;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Panta.Fetchers.Extensions.UTM
{
    public class UTMCourseFetcher : WebpageItemFetcher<UTCourse>
    {
        public UTMCourseFetcher() : base(Home) { }
        private static Regex DepartmentRegex, SelectRegex;
        private const string Home = WebUrlConstants.UTMHome;
        private const string Address = WebUrlConstants.UTMFormat;
        private const string Session = WebUrlConstants.UTMSession;

        static UTMCourseFetcher()
        {
            SelectRegex = new Regex("<select name='subjectarea'.*?</select>");
            DepartmentRegex = new Regex("<option value='(?<index>[0-9]+)'>(?<depName>[^<]*)");
        }

        public override IEnumerable<UTCourse> FetchItems()
        {
            List<UTCourse> results = new List<UTCourse>();

            this.Content = this.Content.Replace("\n", String.Empty).Replace("\r", String.Empty);
            this.Content = SelectRegex.Match(this.Content).Value;
            MatchCollection departments = DepartmentRegex.Matches(this.Content);
            //Parallel.ForEach<Match>(departments.Cast<Match>(), new ParallelOptions() { MaxDegreeOfParallelism = Environment.ProcessorCount }, delegate(Match dep)
            foreach (Match dep in departments)
            {
                UTMCourseInfoFetcher fetcher = new UTMCourseInfoFetcher(dep.Groups["depName"].Value.Trim(' '), String.Format(Address, dep.Groups["index"].Value, Session));
                IEnumerable<UTCourse> result = fetcher.FetchItems();
                lock (this)
                {
                    results.AddRange(result);
                }
            }
            //);
            return results;
        }
    }
}
