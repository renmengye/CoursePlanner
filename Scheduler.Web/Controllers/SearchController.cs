using Panta.DataModels;
using Panta.Searching;
using Scheduler.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Http;

namespace Scheduler.Web.Controllers
{
    public class SearchController : ApiController
    {
        public CourseSearchResult Get(string q)
        {
            q = HttpUtility.HtmlDecode(q);
            CourseSearchPresenter presenter = (CourseSearchPresenter)HttpContext.Current.Application["Presenter"];
            return presenter.GetCourseList(q);
        }

        public Course Get(int id)
        {
            CourseSearchPresenter presenter = (CourseSearchPresenter)HttpContext.Current.Application["Presenter"];
            IEnumerable<Course> result = presenter.GetCoursesFromIDs(presenter.GetCourseIDMatches("id:" + id));
            return result.FirstOrDefault<Course>();
        }
    }
}
