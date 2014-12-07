using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Panta.Fetchers.Extensions.UT
{
    public class WebUrlConstants
    {
        public const string EngTimetableFall = "http://www.apsc.utoronto.ca/timetable/fall.html";
        public const string EngTimetableWinter = "http://www.apsc.utoronto.ca/timetable/winter.html";
        public const string EngCalendar = "http://www.apsc.utoronto.ca/Calendars/2014-2015/Course_Descriptions.html";

        public const string EngPrograms = "http://www.apsc.utoronto.ca/Calendars/2014-2015/Engineering_Programs.html";

        public const string ArtsciTimetableRoot = "http://www.artsandscience.utoronto.ca/ofr/timetable/winter/";
        public const string ArtsciTimetableHome = "sponsors.htm";
        public const string ArtsciCourseDetailRoot = "http://www.artsandscience.utoronto.ca/ofr/calendar/crs_";
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

        public const string UTSCTimetable = "http://www.utsc.utoronto.ca/~registrar/timetable_src/export.php?&submit&course&sess=year";
        public const string UTSCDepartment = "http://www.utsc.utoronto.ca/~registrar/scheduling/timetable";

        public const string UTMHome = "https://registrar.utm.utoronto.ca/student/timetable/index.php";
        public const string UTMFormat = "https://registrar.utm.utoronto.ca/student/timetable/formatCourses2.php?yos=0&subjectarea={0}&session={1}&course=&instr_sname=";
        public const string UTMSession = "20149";
    }
}
