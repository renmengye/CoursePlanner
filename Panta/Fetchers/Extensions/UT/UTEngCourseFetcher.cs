using Panta.DataModels;
using Panta.DataModels.Extensions.UT;
using Panta.Indexing.Extensions.UT;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Panta.Fetchers.Extensions.UT
{
    public class UTEngCourseFetcher : IItemFetcher<UTCourse>
    {
        private Dictionary<string, UTCourse> CoursesCollection { get; set; }
        private static Regex CodeRegex;

        static UTEngCourseFetcher()
        {
            CodeRegex = new Regex("(?<code>[A-Z]{3}[0-9]{3})(?<prefix>[HY][1])", RegexOptions.Compiled);
        }

        public UTEngCourseFetcher()
        {
            this.CoursesCollection = new Dictionary<string, UTCourse>();
        }

        public IEnumerable<UTCourse> FetchItems()
        {
            IItemFetcher<UTCourse> fallCourseFetcher = new UTEngCourseInfoFetcher(@"http://www.apsc.utoronto.ca/evss/fall.html");
            IItemFetcher<UTCourse> winterCourseFetcher = new UTEngCourseInfoFetcher(@"http://www.apsc.utoronto.ca/evss/winter.html");
            IEnumerable<UTCourse> allCourses = fallCourseFetcher.FetchItems().Concat<UTCourse>(winterCourseFetcher.FetchItems());
            IEnumerable<UTCourse> coursesDetail = new UTEngCourseDetailFetcher(@"http://www.apsc.utoronto.ca/Calendars/2014-2015/Course_Descriptions.html").FetchItems();

            // Merge course info and course detail
            allCourses = allCourses.GroupJoin(coursesDetail,
                    (x => x.Abbr),
                    (x => x.Abbr),
                    ((x, y) => this.CombineInfoDetail(x, y.FirstOrDefault())),
                    new UTCourseAbbrComparer());

            UTEngHssCsChecker checker = new UTEngHssCsChecker();
            foreach (UTCourse course in allCourses)
            {
                course.Faculty = "Engineering";

                if (checker.CheckEngCs(course.Code))
                {
                    course.AddCategory("cs");
                }

                if (!CoursesCollection.ContainsKey(course.Abbr))
                {
                    CoursesCollection.Add(course.Abbr, course);
                }
            }


            // Match the prerequisites to postrequisites
            foreach (UTCourse course in CoursesCollection.Values)
            {
                if (!String.IsNullOrEmpty(course.Prerequisites))
                {
                    foreach (Match match in CodeRegex.Matches(course.Prerequisites))
                    {
                        string abbr = match.Value;
                        TryMatchPreq(this.CoursesCollection, course.Abbr, abbr, "F");
                        TryMatchPreq(this.CoursesCollection, course.Abbr, abbr, "S");
                        TryMatchPreq(this.CoursesCollection, course.Abbr, abbr, "Y");
                    }
                }
            }

            return CoursesCollection.Values;
        }

        private UTCourse CombineInfoDetail(UTCourse info, UTCourse detail)
        {
            if (detail != null)
            {
                info.Name = detail.Name;
                info.Description = detail.Description;
                info.Corequisites = detail.Corequisites;
                info.Prerequisites = detail.Prerequisites;
                info.Exclusions = detail.Exclusions;
                info.DistributionRequirement = detail.DistributionRequirement;
                info.Program = detail.Program;
            }
            return info;
        }

        private bool TryMatchPreq(Dictionary<string, UTCourse> coursesCollection, string courseAbbr, string searchAbbr, string semester)
        {
            UTCourse preqCourse;

            if (coursesCollection.TryGetValue(searchAbbr + semester, out preqCourse))
            {
                if (String.IsNullOrEmpty(preqCourse.Postrequisites))
                {
                    preqCourse.Postrequisites = courseAbbr;
                }
                else
                {
                    preqCourse.Postrequisites = String.Join(" ", preqCourse.Postrequisites, courseAbbr);
                }
                return true;
            }
            return false;
        }
    }
}
