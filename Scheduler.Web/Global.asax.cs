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
        private const string CoursesIndexFileName = @"uoft_courses.idx";
        private const string CoursesFileName = @"uoft_courses.bin";

        private const string ProgramsIndexFileName = @"uoft_progs.idx";
        private const string ProgramsFileName = @"uoft_progs.bin";

        internal IIndexablePresenter<Course> CoursePresenter
        {
            get
            {
                return (IIndexablePresenter<Course>)Application["CoursePresenter"];
            }
            set
            {
                Application["CoursePresenter"] = value;
            }
        }
        internal DefaultIIndexableCollection<Course> UOfTCourses
        {
            get
            {
                return (DefaultIIndexableCollection<Course>)Application["Courses"];
            }
            set
            {
                Application["Courses"] = value;
            }
        }
        internal InvertedWordIndex CourseIndex
        {
            get
            {
                return (InvertedWordIndex)Application["CourseIndex"];
            }
            set
            {
                Application["CourseIndex"] = value;
            }
        }
        internal IIndexablePresenter<SchoolProgram> ProgramPresenter
        {
            get
            {
                return (IIndexablePresenter<SchoolProgram>)Application["ProgramPresenter"];
            }
            set
            {
                Application["ProgramPresenter"] = value;
            }
        }
        internal DefaultIIndexableCollection<SchoolProgram> UOfTPrograms
        {
            get
            {
                return (DefaultIIndexableCollection<SchoolProgram>)Application["Programs"];
            }
            set
            {
                Application["Programs"] = value;
            }
        }
        internal InvertedWordIndex ProgramIndex
        {
            get
            {
                return (InvertedWordIndex)Application["ProgramIndex"];
            }
            set
            {
                Application["ProgramIndex"] = value;
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

            this.UOfTCourses = DefaultIIndexableCollection<Course>.Read(Path.Combine(path, CoursesFileName));
            this.CourseIndex = InvertedWordIndex.Read(Path.Combine(path, CoursesIndexFileName));
            this.CoursePresenter = new CourseSearchPresenter(this.CourseIndex, this.UOfTCourses);

            this.UOfTPrograms = DefaultIIndexableCollection<SchoolProgram>.Read(Path.Combine(path, ProgramsFileName));
            this.ProgramIndex = InvertedWordIndex.Read(Path.Combine(path, ProgramsIndexFileName));
            this.ProgramPresenter = new ProgramSearchPresenter(this.ProgramIndex, this.UOfTPrograms);
            
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