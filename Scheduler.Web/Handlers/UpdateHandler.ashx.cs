using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;

namespace Scheduler.Web.Handlers
{
    /// <summary>
    /// Shows the last update time of data file
    /// </summary>
    public class UpdateHandler : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            FileInfo dataInfo = new FileInfo(context.Request.PhysicalApplicationPath + "Data\\uoft_courses.bin");
            context.Response.Write("Data updated on " + dataInfo.LastWriteTime.ToShortDateString());
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}