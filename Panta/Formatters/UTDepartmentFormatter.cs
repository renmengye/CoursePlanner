using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Text.RegularExpressions;
using Panta.DataModels;
using Panta.Indexing;

namespace Panta.Formatters
{
    [Serializable]
    public class UTDepartmentFormatter : IWebpageFormatter<Department>
    {
        public string Root = "http://www.artsandscience.utoronto.ca/ofr/timetable/winter/";
        public string Home = "sponsors.htm";
        public string Url { get { return Root + Home; } }
        public string CourseDetailRoot = "http://www.artsandscience.utoronto.ca/ofr/calendar/crs_";

        public IEnumerable<Department> Read()
        {
            List<Department> results = new List<Department>();
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
            Regex departmentRegex = new Regex("<li><a href=[\"]?(?<address>[^\"<]*)[\"]?>(?<name>[^\u005b]*)[\u005b](?<abbr>[A-Z]{3})\u0020courses.*?</a>", RegexOptions.Multiline | RegexOptions.IgnoreCase | RegexOptions.Compiled);
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

                Department dep = new UTDepartment(name, abbr, new UTCourseFormatter(Root + address), new UTCourseDetailFormatter(CourseDetailRoot + abbr.ToLowerInvariant() + ".htm"));

                Console.WriteLine();
                Console.Write("\nDepartment: {0}", dep.Name);

                results.Add(dep);
            }
            return results;
        }
    }
}
