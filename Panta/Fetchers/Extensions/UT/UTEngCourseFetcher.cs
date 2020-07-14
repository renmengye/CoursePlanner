using Panta.DataModels;
using Panta.DataModels.Extensions.UT;
using Panta.Indexing.Extensions.UT;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

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
            IItemFetcher<UTCourse> fallCourseFetcher = new UTEngCourseInfoFetcher(WebUrlConstants.EngTimetableFall);
            IItemFetcher<UTCourse> winterCourseFetcher = new UTEngCourseInfoFetcher(WebUrlConstants.EngTimetableWinter);
            IEnumerable<UTCourse> allCourses = fallCourseFetcher.FetchItems().Concat<UTCourse>(winterCourseFetcher.FetchItems());

            //IEnumerable<UTCourse> courseDetail = new UTEngCourseDetailFetcher(WebUrlConstants.EngCalendar).FetchItems();
            List<UTCourse> courseDetail = new List<UTCourse>();
            Parallel.ForEach<UTCourse>(allCourses, new ParallelOptions() { MaxDegreeOfParallelism = Environment.ProcessorCount }, delegate (UTCourse course)
            {
                string session;
                if (String.Equals(course.Semester, "S")) {
                    session = WebUrlConstants.ArtsciSessionWinter;
                }
                else
                {
                    session = WebUrlConstants.ArtsciSession;
                }
                IEnumerable<UTCourse> courseDetail_ = new UTCourseDetailFetcher(String.Format(WebUrlConstants.CourseFinderCourse, course.Abbr, session)).FetchItems();
                lock (this)
                {
                    if (courseDetail_.Count() > 0)
                    {
                        UTCourse course_ = courseDetail_.First();
                        courseDetail.Add(course_);
                        Console.WriteLine("Engineering Course Detail: {0} | {1}", course_.Abbr, course.Abbr);
                    }
                    else
                    {
                        Console.WriteLine("Engineering Course Detail Not Found: {0}", course.Abbr);
                    }
                }
            });

            // Merge course info and course detail
            allCourses = allCourses.GroupJoin(courseDetail,
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
