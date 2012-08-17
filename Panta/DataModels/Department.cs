using System;
using System.Collections.Generic;
using Panta.Formatters;

namespace Panta.DataModels
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

        /// <summary>
        /// Course reader
        /// </summary>
        [NonSerialized]
        public IWebpageFormatter<Course> CourseFormatter;

        /// <summary>
        /// Use Course code + Semester code as unique key (we might have a number of course readers to match the results)
        /// Not storing the courses (Let School store the courses)
        /// </summary>
        [NonSerialized]
        public Dictionary<string, Course> Courses;


        public Department(string name, string abbr, IWebpageFormatter<Course> courseReader)
        {
            this.Name = name;
            this.Abbr = abbr;
            this.CourseFormatter = courseReader;
            this.Courses = new Dictionary<string, Course>();
        }

        /// <summary>
        /// Get all the courses from the course formatter and put into the dictionary, use signer to sign ids to courses
        /// </summary>
        public virtual void FetchCourses()
        {
            foreach (Course course in CourseFormatter.Read())
            {
                this.Courses.Add(course.Code + course.Semester, course);
            }
        }
    }
}
