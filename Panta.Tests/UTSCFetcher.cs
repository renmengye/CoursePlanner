using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Panta.Fetchers.Extensions.UTSC;
using System.IO;
using Panta.Fetchers.Extensions.UT;

namespace Panta.Tests
{
    [TestClass]
    public class UTSCFetcher
    {
        [TestMethod]
        public void PostMethod()
        {
            //new UTSCCourseInfoHtmlFetcher("C:\\Users\\renme_000\\Projects\\CoursePlanner\\bin\\Debug\\utsc.html").FetchItems();
            new UTSCCourseInfoHtmlFetcher(WebUrlConstants.UTSCTimetable).FetchItems();
            Assert.IsTrue(true);
        }

        //[TestMethod]
        //public void Department()
        //{
        //    new UTSCDepartmentFetcher("http://www.utsc.utoronto.ca/~registrar/scheduling/timetable").FetchItems();
        //}

        //[TestMethod]
        //public void Detail()
        //{
        //    new UTSCCourseDetailFetcher("http://www.utsc.utoronto.ca/~registrar/calendars/calendar/Computer_Science.html").FetchItems();
        //}

        //[TestMethod]
        //public void Program()
        //{
        //    new UTSCProgramDetailFetcher("http://www.utsc.utoronto.ca/~registrar/calendars/calendar/Computer_Science.html").FetchItems();
        //}

        //[TestMethod]
        //public void Course()
        //{
        //    new UTSCCourseFetcher().FetchItems();
        //}

        //[TestMethod]
        //public void AllProgram()
        //{
        //    new UTSCProgramFetcher().FetchItems();
        //}

        //[TestMethod]
        //public void MyTestMethod()
        //{
        //    File.Delete(@"C:\Projects\griddy\Scheduler.Web\Data\uoft_courses.bin");
        //    File.Delete(@"C:\Projects\griddy\Scheduler.Web\Data\uoft_courses.idx");
        //    File.Delete(@"C:\Projects\griddy\Scheduler.Web\Data\uoft_progs.bin");
        //    File.Delete(@"C:\Projects\griddy\Scheduler.Web\Data\uoft_progs.idx");
        //    File.Copy(@"C:\Projects\griddy\bin\Debug\uoft_courses.bin", @"C:\Projects\griddy\Scheduler.Web\Data\uoft_courses.bin");
        //    File.Copy(@"C:\Projects\griddy\bin\Debug\uoft_courses.idx", @"C:\Projects\griddy\Scheduler.Web\Data\uoft_courses.idx");
        //    File.Copy(@"C:\Projects\griddy\bin\Debug\uoft_progs.bin", @"C:\Projects\griddy\Scheduler.Web\Data\uoft_progs.bin");
        //    File.Copy(@"C:\Projects\griddy\bin\Debug\uoft_progs.idx", @"C:\Projects\griddy\Scheduler.Web\Data\uoft_progs.idx");
        //}
    }
}
