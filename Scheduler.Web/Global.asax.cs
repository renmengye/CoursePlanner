using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Panta.Searching;
using Panta.DataModels;
using Panta.Indexing;
using Panta.DataModels.Extensions.UT;
using System.IO;
using System.Threading;
using Scheduler.Common;

namespace Scheduler.Web
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        private const string IndexFileName = @"uoft.idx";
        private const string SchoolFileName = @"uoft.bin";

        internal CourseSearchPresenter Presenter
        {
            get
            {
                return (CourseSearchPresenter)Application["Presenter"];
            }
            set
            {
                Application["Presenter"] = value;
            }
        }
        internal School UOfT
        {
            get
            {
                return (School)Application["School"];
            }
            set
            {
                Application["School"] = value;
            }
        }
        internal InvertedWordIndex Index
        {
            get
            {
                return (InvertedWordIndex)Application["Index"];
            }
            set
            {
                Application["Index"] = value;
            }
        }
        private Thread StateThread;

        protected void Application_Start()
        {
            RouteTable.Routes.MapHttpRoute(
            name: "DefaultApi",
            routeTemplate: "api/{controller}/{id}",
            defaults: new { id = System.Web.Http.RouteParameter.Optional }
            );

            string path = Server.MapPath("Data");

            this.UOfT = School.Read(Path.Combine(path, SchoolFileName));
            this.Index = InvertedWordIndex.Read(Path.Combine(path, IndexFileName));
            this.Presenter = new CourseSearchPresenter(this.Index, this.UOfT);
            
            // Start a background thread to load and maintain the index, user map, and history
            this.StateThread = new Thread(new ThreadStart(MaintainState));
            this.StateThread.Start();
        }

        protected void MaintainState()
        {
            while (true)
            {
                // Wait 15 seconds to re-check. Ask the GC to free up unused memory
                Thread.Sleep(75000);
                GC.Collect();
                Thread.Sleep(75000);
            }
        }
    }
}