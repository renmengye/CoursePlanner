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
    public class CourseController : ApiController
    {
        public SearchResult Get(string q)
        {
            q = HttpUtility.HtmlDecode(q);
            IIndexablePresenter<Course> presenter = (IIndexablePresenter<Course>)HttpContext.Current.Application["CoursePresenter"];
            return presenter.GetItemList(q);
        }

        public Course Get(int id)
        {
            IIndexablePresenter<Course> presenter = (IIndexablePresenter<Course>)HttpContext.Current.Application["CoursePresenter"];
            IEnumerable<Course> result = presenter.GetItemsFromIDs(presenter.GetIDMatches("id:" + id));
            return result.FirstOrDefault<Course>();
        }
    }
}
