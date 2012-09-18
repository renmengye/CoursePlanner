using Panta.DataModels;
using Panta.Searching;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace Scheduler.Web.Controllers
{
    public class ProgramController : ApiController
    {
        public SearchResult Get(string q)
        {
            q = HttpUtility.HtmlDecode(q);
            IIndexablePresenter<SchoolProgram> presenter = (IIndexablePresenter<SchoolProgram>)HttpContext.Current.Application["ProgramPresenter"];
            return presenter.GetItemList(q);
        }
    }
}
