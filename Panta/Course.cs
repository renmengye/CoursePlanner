using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Panta
{
    [Serializable]
    public class Course
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string Semester { get; set; }

        // use section name as dictionary key
        public Dictionary<string, CourseSection> Sections { get; set; }

        public string Description { get; set; }
        public IEnumerable<string> Prerequisites { get; set; }
        public string DistributionRequirement { get; set; }
        public string BreadthRequirement { get; set; }

        public Course()
        {
            Sections = new Dictionary<string, CourseSection>();
        }

        public void FetchDescription()
        {
        }
    }

    [Serializable]
    public class CourseSection
    {
        // Contains time, location, waitlist, instructor
        public string Time { get; set; }
        public string Name { get; set; }
        public string Location { get; set; }
        public bool WaitList { get; set; }
        public string Instructor { get; set; }
    }
}
