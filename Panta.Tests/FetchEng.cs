using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Panta.Fetchers;
using System.Net;

namespace Panta.Tests
{
    /// <summary>
    /// Summary description for FetchEng
    /// </summary>
    [TestClass]
    public class FetchEng
    {
        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod]
        public void TestMethod1()
        {
            //
            // TODO: Add test logic here
            //
            string url = "http://www.apsc.utoronto.ca/timetable/fall.html";
            using (var client = new TimeoutClient(10000))
            {
                try
                {
                    string content = client.DownloadString(url);
                    Console.WriteLine(content);
                }
                catch (WebException ex)
                {
                    //Console.WriteLine("Retry: {0:D}, unable to fetch: {1:G}", retryCount, this.Url);
                    Console.WriteLine(ex.ToString());
                }
            }
        }
    }
}
