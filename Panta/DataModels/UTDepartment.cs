using Panta.Formatters;

namespace Panta.DataModels
{
    public class UTDepartment : Department
    {
        /// <summary>
        /// UofT has course details on another webpage so we need to have one more formatter
        /// </summary>
        public IWebpageFormatter<Course> CourseDetailFormatter { get; set; }


        public UTDepartment(string name, string abbr, IWebpageFormatter<Course> courseReader, IWebpageFormatter<Course> courseDetailReader)
            : base(name, abbr, courseReader)
        {
            this.CourseDetailFormatter = courseDetailReader;
        }

        /// <summary>
        /// Fetch all the courses
        /// Add FetchCourseDetail logic to the base class
        /// </summary>
        public override void FetchCourses()
        {
            base.FetchCourses();
            FetchCoursesDetail();
        }

        /// <summary>
        /// Use the course detail formatter to read webpage and try match the existing courses with course codes
        /// </summary>
        private void FetchCoursesDetail()
        {
            foreach (Course course in CourseDetailFormatter.Read())
            {
                TryMatchSemester(course, "Y");
                TryMatchSemester(course, "F");
                TryMatchSemester(course, "S");
            }
        }


        /// <summary>
        /// Try match the existing course in the dictionary with a course that has no semester information
        /// </summary>
        /// <param name="course">Couse without semester information</param>
        /// <param name="semester">A semester of guess</param>
        /// <returns></returns>
        private bool TryMatchSemester(Course course, string semester)
        {
            Course existedCourse;
            if (this.Courses.TryGetValue(course.Code + semester, out existedCourse))
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
