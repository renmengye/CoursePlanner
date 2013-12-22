using Panta.DataModels.Extensions.UT;
using Panta.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using Panta.Indexing.Extensions.UT;

namespace Panta.Fetchers.Extensions.UT
{

    public class UTArtsciCourseFetcher : IItemFetcher<UTCourse>
    {
        /// <summary>
        /// A dictionary stores all the courses fetched from small fetchers and preventing duplicate abbreviation naming for merging
        /// </summary>
        private Dictionary<string, UTCourse> CoursesCollection { get; set; }
        private static Regex CodeRegex;

        static UTArtsciCourseFetcher()
        {
            CodeRegex = new Regex("(?<code>[A-Z]{3}[0-9]{3})(?<prefix>[HY][1])", RegexOptions.Compiled);
        }

        public UTArtsciCourseFetcher()
        {
            this.CoursesCollection = new Dictionary<string, UTCourse>();
        }

        public IEnumerable<UTCourse> FetchItems()
        {
            IItemFetcher<UTDepartment> depFetcher = new UTArtsciDepartmentFetcher();

            List<UTCourse> courses = new List<UTCourse>();
            List<UTCourse> coursesDetail = new List<UTCourse>();
            List<UTDepartment> deps = new List<UTDepartment>(depFetcher.FetchItems());

            // Parallel threads for fetching each department
            Parallel.ForEach<UTDepartment>(depFetcher.FetchItems(), new ParallelOptions() { MaxDegreeOfParallelism = Environment.ProcessorCount }, delegate(UTDepartment dep)
            //foreach (UTDepartment dep in depFetcher.FetchItems())
            {
                IItemFetcher<UTCourse> courseInfoFetcher = new UTArtsciCourseInfoFetcher(dep.Url);
                IItemFetcher<UTCourse> courseDetailFetcher = new UTArtsciCourseDetailFetcher(dep.DetailUrl);

                // Going to merge the info fetched from courseDetail and courseInfo
                IEnumerable<UTCourse> tempCourses = courseInfoFetcher.FetchItems();

                // Add hss/cs/department info
                UTEngHssCsChecker checker = new UTEngHssCsChecker();
                foreach (UTCourse course in tempCourses)
                {
                    // Add department info
                    course.Department = dep.Name;
                    course.Faculty = "Artsci";

                    // Check cs/hss requirement for engineering students
                    if (checker.CheckHss(course.Code + course.SemesterPrefix))
                    {
                        course.AddCategory("hss");
                    }
                    if (checker.CheckArtsciCs(course.Code + course.SemesterPrefix))
                    {
                        course.AddCategory("cs");
                    }
                }

                courses.AddRange(tempCourses);
                coursesDetail.AddRange(courseDetailFetcher.FetchItems());
            });
            //}

            // Merge course info and course detail
            IEnumerable<UTCourse> coursesTotal = courses.GroupJoin(coursesDetail,
                (x => x.Abbr),
                (x => x.Abbr),
                ((x, y) => this.CombineInfoDetail(x, y.FirstOrDefault())),
                new UTCourseAbbrComparer());

            // Add to unique dictionary
            foreach (UTCourse course in coursesTotal)
            {
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
                        string abbr = match.Groups["code"].Value;
                        TryMatchPreq(this.CoursesCollection, course.Abbr, abbr, "H1F");
                        TryMatchPreq(this.CoursesCollection, course.Abbr, abbr, "H1S");
                        TryMatchPreq(this.CoursesCollection, course.Abbr, abbr, "Y1Y");
                        TryMatchPreq(this.CoursesCollection, course.Abbr, abbr, "H1Y");
                    }
                }
            }
            return CoursesCollection.Values;
        }

        private UTCourse CombineInfoDetail(UTCourse info, UTCourse detail)
        {
            if (detail != null)
            {
                info.Description = detail.Description;
                info.Corequisites = detail.Corequisites;
                info.Prerequisites = detail.Prerequisites;
                info.Exclusions = detail.Exclusions;
                info.DistributionRequirement = detail.DistributionRequirement;
                info.BreadthRequirement = detail.BreadthRequirement;
            }
            return info;
        }

        /// <summary>
        /// Try match the prerequisite to the postrequisite course
        /// </summary>
        /// <param name="coursesCollection"></param>
        /// <param name="courseAbbr"></param>
        /// <param name="searchCode"></param>
        /// <param name="semester"></param>
        /// <returns></returns>
        private bool TryMatchPreq(Dictionary<string, UTCourse> coursesCollection, string courseAbbr, string searchCode, string semester)
        {
            UTCourse preqCourse;

            if (coursesCollection.TryGetValue(searchCode + semester, out preqCourse))
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
