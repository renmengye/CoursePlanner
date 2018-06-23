using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Panta.Fetchers.Extensions.UT;
using Panta.Fetchers;
using Panta.DataModels;

namespace Panta.Tests
{
    [TestClass]
    public class ArtsciNew2Tests
    {
        [TestMethod]
        public void TestMethod1()
        {
            string url = @"https://timetable.iit.artsci.utoronto.ca/api/20189/courses?code=A";
            UTArtsciCourseInfoFetcherNew2 fetcher2 = new UTArtsciCourseInfoFetcherNew2(url);
            fetcher2.FetchItems();
        }
    }
}
