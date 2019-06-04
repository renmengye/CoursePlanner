using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Panta.Fetchers.Extensions.UT;

namespace Panta.Tests
{
    [TestClass]
    public class UTArtsciCourseDetailFetcherTests
    {
        [TestMethod]
        public void PageNumber()
        {
            var fetcher = new UTArtsciCourseDetailPageNumberFetcher(
                String.Format(WebUrlConstants.ArtsciCourseDetailNew, 0));
            fetcher.FetchItems();
        }

        [TestMethod]
        public void Basics()
        {
            var fetcher = new UTArtsciCourseDetailFetcherNew(
                String.Format(WebUrlConstants.ArtsciCourseDetailNew, 0));
            fetcher.FetchItems();
        }
    }
}
