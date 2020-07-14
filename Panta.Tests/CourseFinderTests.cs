using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Panta.Fetchers.Extensions.UT;

namespace Panta.Tests
{
    [TestClass]
    public class CourseFinderTests
    {
        [TestMethod]
        public void Basic()
        {
            string url = String.Format(WebUrlConstants.CourseFinderCourse, "MSE490H1S", WebUrlConstants.ArtsciSession);
            UTCourseDetailFetcher fetcher = new UTCourseDetailFetcher(url);
            fetcher.FetchItems();
        }
    }
}
