using Panta.DataModels;
using Panta.Indexing;
using Panta.Searching;
using Scheduler.Common;
using System;
using System.IO;
using System.Threading;
using System.Web.Http;
using System.Web.Routing;

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
        private Thread UpdateThread;

        protected void Application_Start()
        {
            RouteTable.Routes.MapHttpRoute(
            name: "DefaultApi",
            routeTemplate: "api/{controller}/{id}",
            defaults: new { id = System.Web.Http.RouteParameter.Optional }
            );

            string path = Server.MapPath("Data");
            Application["Path"] = path;
            ReadData(path);
            Application["Updating"] = false;
            Application["LastChange"] = new FileInfo(Path.Combine(path, CoursesFileName)).LastWriteTimeUtc;

            // Start a background thread to update data
            this.UpdateThread = new Thread(new ThreadStart(UpdateData));
            this.UpdateThread.Start();

            // Start a background thread to load and maintain the index, user map, and history
            this.StateThread = new Thread(new ThreadStart(MaintainState));
            this.StateThread.Start();
        }

        protected void MaintainState()
        {
            while (true)
            {
                // Wait 15 seconds to re-check. Ask the GC to free up unused memory
                GC.Collect();
                if (!(bool)Application["Updating"])
                {
                    this.CoursePresenter.GetIDMatches("mat", null, null);
                    this.ProgramPresenter.GetIDMatches("mat", null, null);
                }
                Thread.Sleep(150000);
            }
        }

        protected void UpdateData()
        {
            while (true)
            {
                DateTime lastChange = DateTime.MinValue;
                if (Application["LastChange"] != null)
                {
                    lastChange = (DateTime)Application["LastChange"];
                }

                string path = (string)Application["Path"];

                FileInfo file = new FileInfo(Path.Combine(path, CoursesFileName));
                if (file.LastWriteTimeUtc > lastChange)
                {
                    Application["Updating"] = true;
                    ReadData(path);
                    Application["LastChange"] = file.LastWriteTimeUtc;
                    Application["Updating"] = false;
                }

                Thread.Sleep(new TimeSpan(0, 0, 15));
            }
        }

        protected void ReadData(string path)
        {
            this.UOfTCourses = DefaultIIndexableCollection<Course>.ReadBin(Path.Combine(path, CoursesFileName));
            this.CourseIndex = InvertedWordIndex.Read(Path.Combine(path, CoursesIndexFileName));
            this.CoursePresenter = new CourseSearchPresenter(this.CourseIndex, this.UOfTCourses);

            this.UOfTPrograms = DefaultIIndexableCollection<SchoolProgram>.ReadBin(Path.Combine(path, ProgramsFileName));
            this.ProgramIndex = InvertedWordIndex.Read(Path.Combine(path, ProgramsIndexFileName));
            this.ProgramPresenter = new ProgramSearchPresenter(this.ProgramIndex, this.UOfTPrograms);
        }
    }
}