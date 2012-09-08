using Panta.DataModels.Extensions.UT;
using Panta.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Panta.Fetchers
{
    public class UTArtsciCourseFetcher : IItemFetcher<UTCourse>
    {
        /// <summary>
        /// This property is not used here since it uses a couple little item fetchers
        /// </summary>
        public string Url { get; set; }

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

                // Add course info items to a dictionary for matching the same ones with detail items
                Dictionary<string, UTCourse> coursesCollection = new Dictionary<string, UTCourse>();
                foreach (UTCourse course in courses)
                {
                    // Add department info
                    course.Department = dep.Name;
                    string key=course.Abbr + course.SemesterPrefix + course.Semester;
                    if (coursesCollection.ContainsKey(key))
                    {
                        coursesCollection.Add(key, course);
                    }
                    else
                    {
                        Console.WriteLine("Duplicate naming: " + key);
                    }
                }

                // Use a subbody TryMatchSemester to match the courses
                foreach (UTCourse course in coursesDetail)
                {
                    TryMatchSemester(coursesCollection, course, "Y");
                    TryMatchSemester(coursesCollection, course, "F");
                    TryMatchSemester(coursesCollection, course, "S");
                }

                lock (this)
                {
                    results.AddRange(courses);
                }
            });

            return results;
        }

        /// <summary>
        /// Try match the existing course in the dictionary with a course that has no semester information
        /// </summary>
        /// <param name="course">Couse without semester information</param>
        /// <param name="semester">A semester of guess</param>
        /// <returns></returns>
        private bool TryMatchSemester(Dictionary<string, UTCourse> courseCollection, UTCourse course, string semester)
        {
            UTCourse existedCourse;
            if (courseCollection.TryGetValue(course.Abbr + course.SemesterPrefix + semester, out existedCourse))
            {
                existedCourse.Description = course.Description;
                existedCourse.Prerequisites = course.Prerequisites;
                existedCourse.Exclusions = course.Exclusions;
                existedCourse.DistributionRequirement = course.DistributionRequirement;
                existedCourse.BreadthRequirement = course.BreadthRequirement;
                return true;
            }
            return false;
        }
    }
}
