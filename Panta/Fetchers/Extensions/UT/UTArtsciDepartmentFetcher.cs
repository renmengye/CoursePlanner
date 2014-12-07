using Panta.DataModels.Extensions.UT;
using Panta.Fetchers.Extensions.UT;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Text.RegularExpressions;

namespace Panta.Fetchers
{
    [Serializable]
    public class UTArtsciDepartmentFetcher : IItemFetcher<UTDepartment>
    {
        public string Root = WebUrlConstants.ArtsciTimetableRoot;
        public string Home = WebUrlConstants.ArtsciTimetableHome;
        public string Url { get { return Root + Home; } }
        public string CourseDetailRoot = WebUrlConstants.ArtsciCourseDetailRoot;

        public IEnumerable<UTDepartment> FetchItems()
        {
            List<UTDepartment> results = new List<UTDepartment>();
            WebClient client = new WebClient();
            string departmentContent;

            try
            {
                departmentContent = client.DownloadString(Url);
            }
            catch (WebException ex)
            {
                ex.Source = "Unable to fetch: " + Url;
                Trace.WriteLine(ex.ToString());
                return results;
            }

            departmentContent = departmentContent.Replace("\r\n", String.Empty);
            departmentContent = departmentContent.Replace("<a href=csb.html></a>  <a href=\"cine.html\">", "<a href=\"cine.html\">");
            Regex departmentRegex = new Regex("<li><a href=[\"]?(?<address>[^\"<]*)[\"]?>(?<name>[^\u005b]*)[\u005b](?<abbr>[A-Z]{3}).*?</a>", RegexOptions.Multiline | RegexOptions.IgnoreCase | RegexOptions.Compiled);
            MatchCollection matches = departmentRegex.Matches(departmentContent);

            foreach (Match match in matches)
            {
                string name = match.Groups["name"].ToString();
                string address = match.Groups["address"].ToString();
                string abbr = match.Groups["abbr"].ToString();

                name = name.Replace("\r", String.Empty).Replace("\n", String.Empty).Replace("&amp;", "&").TrimEnd(' ');
                Regex circleRegex = new Regex("[\u0020]*[\u0028][^\u0029]*[\u0029][\u0020]*", RegexOptions.Compiled);
                Regex doubleSpace = new Regex("([\u0020]{2})", RegexOptions.Compiled);
                name = circleRegex.Replace(name, String.Empty);
                name = doubleSpace.Replace(name, " ");

                //UTDepartment dep = new UTDepartment(name, abbr, new UTCourseFetcher(Root + address), new UTCourseDetailFormatter(CourseDetailRoot + abbr.ToLowerInvariant() + ".htm"));
                UTDepartment dep = new UTDepartment()
                {
                    Name = name,
                    Abbr = abbr,
                    Url = Root + address,
                    DetailUrl = CourseDetailRoot + abbr.ToLowerInvariant() + ".htm"
                };

                Console.WriteLine("Department: {0}", dep.Name);

                results.Add(dep);
            }

            return results;
        }
    }
}
