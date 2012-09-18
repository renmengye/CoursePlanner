using Panta.DataModels.Extensions.UT;
using Panta.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

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
            List<UTCourse> results = new List<UTCourse>();

            IItemFetcher<UTDepartment> depFetcher = new UTArtsciDepartmentFetcher();

            // Parallel threads for fetching each department
            Parallel.ForEach<UTDepartment>(depFetcher.FetchItems(), new ParallelOptions() { MaxDegreeOfParallelism = Environment.ProcessorCount }, delegate(UTDepartment dep)
            //foreach (UTDepartment dep in depFetcher.FetchItems())
            {
                IItemFetcher<UTCourse> courseInfoFetcher = new UTArtsciCourseInfoFetcher(dep.Url);
                IItemFetcher<UTCourse> courseDetailFetcher = new UTArtsciCourseDetailFetcher(dep.DetailUrl);

                // Going to merge the info fetched from courseDetail and courseInfo
                IEnumerable<UTCourse> courses = courseInfoFetcher.FetchItems();
                IEnumerable<UTCourse> coursesDetail = courseDetailFetcher.FetchItems();

                foreach (UTCourse course in courses)
                {
                    // Add department info
                    course.Department = dep.Name;
                    course.Faculty = "Artsci";

                    lock (this)
                    {
                        if (!CoursesCollection.ContainsKey(course.Abbr))
                        {
                            CoursesCollection.Add(course.Abbr, course);
                        }
                        else
                        {
                            //Console.WriteLine("Duplicate naming: " + course.Abbr);
                        }
                    }
                }

                // Use a subbody TryMatchSemester to match the courses
                foreach (UTCourse course in coursesDetail)
                {
                    TryMatchSemester(CoursesCollection, course, "Y");
                    TryMatchSemester(CoursesCollection, course, "F");
                    TryMatchSemester(CoursesCollection, course, "S");
                }
            });
            //}

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

        /// <summary>
        /// Try match the existing course in the dictionary with a course that has no semester information
        /// </summary>
        /// <param name="course">Couse without semester information</param>
        /// <param name="semester">A semester of guess</param>
        /// <returns></returns>
        private bool TryMatchSemester(Dictionary<string, UTCourse> coursesCollection, UTCourse course, string semester)
        {
            UTCourse existedCourse;

            lock (this)
            {
                if (coursesCollection.TryGetValue(course.Code + course.SemesterPrefix + semester, out existedCourse))
                {
                    existedCourse.Description = course.Description;
                    existedCourse.Corequisites = course.Corequisites;
                    existedCourse.Prerequisites = course.Prerequisites;
                    existedCourse.Exclusions = course.Exclusions;
                    existedCourse.DistributionRequirement = course.DistributionRequirement;
                    existedCourse.BreadthRequirement = course.BreadthRequirement;
                    return true;
                }
            }
            return false;
        }


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
