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
            UTArtsciCourseInfoFetcherNew2 fetcher = new UTArtsciCourseInfoFetcherNew2(
               @"https://timetable.iit.artsci.utoronto.ca/api/courses?code=A");
            fetcher.FetchItems();
        }
    }
}
