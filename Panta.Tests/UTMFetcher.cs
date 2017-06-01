using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Panta.Fetchers.Extensions.UTM;

namespace Panta.Tests
{
    [TestClass]
    public class UTMFetcher
    {
        [TestMethod]
        public void CourseInfoFecther()
        {
            UTMCourseInfoFetcher fecther = new UTMCourseInfoFetcher("xxx", "https://registrar2.utm.utoronto.ca/student/timetable/formatCourses.php?viewall=&yos=0&subjectarea=33&session=20139&course=&instr_sname=");
            fecther.FetchItems();
        }

        [TestMethod]
        public void CourseFetcher()
        {
            UTMCourseFetcher fetcher = new UTMCourseFetcher();
            fetcher.FetchItems(); 
        }

        [TestMethod]
        public void ProgramUrl()
        {
            new UTMProgramUrlFetcher("https://registrar.utm.utoronto.ca/student/calendar/program_list.pl").FetchItems();
        }

        [TestMethod]
        public void ProgramDetail()
        {
            new UTMProgramDetailFetcher("https://registrar.utm.utoronto.ca/student/calendar/program_group.pl?Group_Id=41").FetchItems();
        }

        [TestMethod]
        public void AllPrograms()
        {
            new UTMProgramFetcher().FetchItems();
        }
    }
}
