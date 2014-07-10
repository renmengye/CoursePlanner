using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Panta.Fetchers.Extensions.UT;

namespace Panta.Tests
{
    [TestClass]
    public class UTEngFetcherTest
    {
        [TestMethod]
        public void TestMethod1()
        {
        //    UTEngCourseInfoFetcher fetcher = new UTEngCourseInfoFetcher(@"http://www.apsc.utoronto.ca/timetable/fall.html");
        //    fetcher.FetchItems();
            //UTEngHssFetcher fetcher = new UTEngHssFetcher("http://www.undergrad.engineering.utoronto.ca/Office_of_the_Registrar/Electives/HSS_Electives.htm");
            //var a = fetcher.FetchItems();
            UTArtsciCourseInfoFetcher fetcher = new UTArtsciCourseInfoFetcher("http://www.artsandscience.utoronto.ca/ofr/timetable/winter/cine.html");
            var a = fetcher.FetchItems();
        }

        [TestMethod]
        public void MyTestMethod2()
        {

            UTArtsciCourseInfoFetcher fetcher = new UTArtsciCourseInfoFetcher("http://www.artsandscience.utoronto.ca/ofr/timetable/winter/chm.html");
            var a = fetcher.FetchItems();
        }

        [TestMethod]
        public void MyTestMethod()
        {
            //UTEngCourseFetcher fetcher = new UTEngCourseFetcher();
            //fetcher.FetchItems();
        }
    }
}
