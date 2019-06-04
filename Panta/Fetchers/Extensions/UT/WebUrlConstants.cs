using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Panta.Fetchers.Extensions.UT
{
    public class WebUrlConstants
    {
        public const string EngTimetableFall = "https://portal.engineering.utoronto.ca/sites/timetable/fall.html";
        public const string EngTimetableWinter = "https://portal.engineering.utoronto.ca/sites/timetable/winter.html";
        public const string EngCalendar = "https://portal.engineering.utoronto.ca/sites/calendars/current/Course_Descriptions.html";
        public const string EngPrograms = "https://portal.engineering.utoronto.ca/sites/calendars/current/Engineering_Programs.html";
        public const string ArtsciTimetableRoot = "http://www.artsandscience.utoronto.ca/ofr/timetable/winter/";
        public const string ArtsciTimetableHome = "index.html";
        public const string ArtsciCourseDetailRoot = "http://www.artsandscience.utoronto.ca/ofr/calendar/crs_";
        public const string ArtsciSession = "20199";
        public static string ArtsciTimetableNew = String.Format("https://timetable.iit.artsci.utoronto.ca/api/{0}/courses?code=", ArtsciSession);
        public const string ArtsciCourseDetailNew = "https://fas.calendar.utoronto.ca/search-courses?combine=&field_breadth_req_value=All&field_distribution_req_value=All&field_fas_program_area_value=All&filter_2=&page={0}";
        public static string[] ArtsciSeminars =  { 
                                              "http://www.artsandscience.utoronto.ca/ofr/1213_199/ccr199h1.html",
                                              "http://www.artsandscience.utoronto.ca/ofr/1213_199/ccr199y1.html",
                                              "http://www.artsandscience.utoronto.ca/ofr/1213_199/lte199h1.html",
                                              "http://www.artsandscience.utoronto.ca/ofr/1213_199/lte199y1.html",
                                              "http://www.artsandscience.utoronto.ca/ofr/1213_199/pmu199h1.html",
                                              "http://www.artsandscience.utoronto.ca/ofr/1213_199/pmu199y1.html",
                                              "http://www.artsandscience.utoronto.ca/ofr/1213_199/sii199h1.html",
                                              "http://www.artsandscience.utoronto.ca/ofr/1213_199/sii199y1.html",
                                              "http://www.artsandscience.utoronto.ca/ofr/1213_199/tbb199h1.html",
                                              "http://www.artsandscience.utoronto.ca/ofr/1213_199/tbb199y1.html",
                                              "http://www.artsandscience.utoronto.ca/ofr/1213_199/xbc199y1.html"
                                          };

        public const string UTSCTimetable = "https://www.utsc.utoronto.ca/~registrar/scheduling/timetable";
        // public const string UTSCTimetable = "http://www.utsc.utoronto.ca/~registrar/timetable_src/export.php?&submit&course&sess=year";
        public const string UTSCDepartment = "http://www.utsc.utoronto.ca/~registrar/calendars/calendar/Programs.html";
        public const string UTSCSession = "year";

        //public const string UTMHome = "https://registrar.utm.utoronto.ca/student/timetable/index.php";
        //public const string UTMFormat = "https://registrar.utm.utoronto.ca/student/timetable/formatCourses2.php?yos=0&subjectarea={0}&session={1}&course=&instr_sname=";
        public const string UTMFormat = "https://student.utm.utoronto.ca/timetable/timetable?subjectarea={0}&session={1}";
        public const string UTMSession = "20199";
        public const string UTMDepartment = "https://student.utm.utoronto.ca/timetable/api/departments";
        //public const string UTMDepartment = "C:\\Users\\renme_000\\Projects\\CoursePlanner\\RawData\\departments.json";
    }
}
