using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Net;

namespace Panta
{
    /// <summary>
    /// A department that belongs to a school
    /// Has a number of courses
    /// </summary>
    [Serializable]
    public class Department
    {
        public string Name { get; set; }
        public string Abbr { get; set; }

        // Root url is get from the school main page
        public string RootUrl { get; set; }

        // Need to combine with root to visit course main page
        public string Url { get; set; }

        public IFormatReader<Course> CourseReader { get; set; }
        public IFormatReader<Course> CourseDetailReader { get; set; }

        // use Course code + Semester code as unique key
        public Dictionary<string, Course> Courses { get; set; }

        // A separate address to fetch course description
        public string CoursesDetailUrl { get; set; }

        public Department(string name, string root, IFormatReader<Course> courseReader, IFormatReader<Course> courseDetailReader)
        {
            this.Name = name;
            this.RootUrl = root;
            this.Courses = new Dictionary<string, Course>();
            this.CourseReader = courseReader;
            this.CourseDetailReader = courseDetailReader;
        }

        public void FetchCourses()
        {
            foreach (Course course in CourseReader.Read())
            {
                this.Courses.Add(course.Code + course.Semester, course);
            }
        }

        public void FetchCoursesDetail()
        {
            foreach (Course course in CourseDetailReader.Read())
            {
                TryMatchSemester(course, "Y");
                TryMatchSemester(course, "F");
                TryMatchSemester(course, "S");
            }
        }

        private bool TryMatchSemester(Course course, string semester)
        {
            Course existedCourse;
            if (this.Courses.TryGetValue(course.Code + semester, out existedCourse))
            {
                existedCourse.Description = course.Description;
                existedCourse.Prerequisites = course.Prerequisites;
                existedCourse.DistributionRequirement = course.DistributionRequirement;
                existedCourse.BreadthRequirement = course.BreadthRequirement;
                return true;
            }
            return false;
        }
    }
}
