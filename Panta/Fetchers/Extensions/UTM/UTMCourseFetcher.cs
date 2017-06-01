using Newtonsoft.Json;
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
        private const string Home = WebUrlConstants.UTMDepartment;
        private const string Address = WebUrlConstants.UTMFormat;
        private const string Session = WebUrlConstants.UTMSession;

        static UTMCourseFetcher()
        {
            //DepartmentRegex = new Regex("<div class=\"option\" data-selectable=\"\" data-value=\"(?<index>[0-9]+)\">(?<depName>[^<]*)</div>");
            //SelectRegex = new Regex("<select name='subjectarea'.*?</select>");
            //SelectRegex = new Regex("<select name='subjectarea'.*?</select>");
            //DepartmentRegex = new Regex("<option value='(?<index>[0-9]+)'>(?<depName>[^<]*)");
        }

        public override IEnumerable<UTCourse> FetchItems()
        {
            List<UTCourse> results = new List<UTCourse>();

            dynamic allDepData = JsonConvert.DeserializeObject(this.Content);
            //Parallel.ForEach<Dynamic>(allDepData, new ParallelOptions() { MaxDegreeOfParallelism = Environment.ProcessorCount }, delegate(dynamic dep)
            foreach (dynamic dep in allDepData)
            {
                UTMCourseInfoFetcher fetcher = new UTMCourseInfoFetcher(dep["deptName"].Value.Trim(' '), String.Format(Address, dep["deptId"].Value, Session));
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
