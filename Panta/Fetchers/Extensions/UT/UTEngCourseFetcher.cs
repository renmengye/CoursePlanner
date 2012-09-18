using Panta.DataModels;
using Panta.DataModels.Extensions.UT;
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
            IItemFetcher<UTCourse> fallCourseFetcher = new UTEngCourseInfoFetcher(@"http://www.apsc.utoronto.ca/timetable/fall.html");
            IItemFetcher<UTCourse> winterCourseFetcher = new UTEngCourseInfoFetcher(@"http://www.apsc.utoronto.ca/timetable/winter.html");
            IEnumerable<UTCourse> allCourses = fallCourseFetcher.FetchItems().Concat<UTCourse>(winterCourseFetcher.FetchItems());

            foreach (UTCourse course in allCourses)
            {
                course.Faculty = "Engineering";

                if (!CoursesCollection.ContainsKey(course.Abbr))
                {
                    CoursesCollection.Add(course.Abbr, course);
                }
                else
                {
                    //Console.WriteLine("Duplicate naming: " + course.Abbr);
                }
            }

            IEnumerable<UTCourse> coursesDetail = new UTEngCourseDetailFetcher(@"http://www.apsc.utoronto.ca/Calendars/2012-2013/Course_Descriptions.html").FetchItems();

            foreach (UTCourse detail in coursesDetail)
            {
                TryMatchSemester(CoursesCollection, detail, "Y");
                TryMatchSemester(CoursesCollection, detail, "F");
                TryMatchSemester(CoursesCollection, detail, "S");
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

        /// <summary>
        /// Try match the existing course in the dictionary with a course that has no semester information
        /// </summary>
        /// <param name="course">Couse without semester information</param>
        /// <param name="semester">A semester of guess</param>
        /// <returns></returns>
        private bool TryMatchSemester(Dictionary<string, UTCourse> coursesCollection, UTCourse course, string semester)
        {
            UTCourse existedCourse;

            if (coursesCollection.TryGetValue(course.Code + course.SemesterPrefix + semester, out existedCourse))
            {
                existedCourse.Name = course.Name;
                existedCourse.Description = course.Description;
                existedCourse.Corequisites = course.Corequisites;
                existedCourse.Prerequisites = course.Prerequisites;
                existedCourse.Exclusions = course.Exclusions;
                existedCourse.DistributionRequirement = course.DistributionRequirement;
                existedCourse.Program = course.Program;
                return true;
            }
            return false;
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
