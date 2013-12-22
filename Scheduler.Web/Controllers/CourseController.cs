using Panta.DataModels;
using Panta.DataModels.Extensions.UT;
using Panta.Searching;
using Scheduler.Common;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Http;

namespace Scheduler.Web.Controllers
{
    public class CourseController : ApiController
    {
        private static Regex CodeRegex;

        static CourseController()
        {
            CodeRegex = new Regex("(?<code>[A-Z]{3,4}[0-9]{2,3})(?<prefix>[HY][0-9])", RegexOptions.Compiled);
        }

        public SearchResult Get(string q)
        {
            if (!(bool)HttpContext.Current.Application["Updating"])
            {
                q = HttpUtility.HtmlDecode(q);
                IIndexablePresenter<Course> presenter = (IIndexablePresenter<Course>)HttpContext.Current.Application["CoursePresenter"];
                return presenter.GetItemList(q);
            }
            return null;
        }

        public IEnumerable<SimpleCourse> Get(string q, bool raw)
        {
            if (!(bool)HttpContext.Current.Application["Updating"])
            {
                q = HttpUtility.HtmlDecode(q);
                IIndexablePresenter<Course> presenter = (IIndexablePresenter<Course>)HttpContext.Current.Application["CoursePresenter"];
                return (presenter as CourseSearchPresenter).GetRawList(q);
            }
            return null;
        }

        public Course Get(uint id)
        {
            if (!(bool)HttpContext.Current.Application["Updating"])
            {
                IIndexableCollection<Course> courses = (IIndexableCollection<Course>)HttpContext.Current.Application["Courses"];
                Course result;
                if (courses.TryGetItem(id, out result))
                {
                    if (result is UTCourse)
                    {
                        UTCourse utResult = result as UTCourse;
                        utResult.Prerequisites = ReplaceCourseCode(utResult.Prerequisites);
                        utResult.Corequisites = ReplaceCourseCode(utResult.Corequisites);
                        utResult.Exclusions = ReplaceCourseCode(utResult.Exclusions);
                        return utResult;
                    }
                }
            }
            return null;
        }

        private static string ReplaceCourseCode(string content)
        {
            if (content != null)
            {
                return CodeRegex.Replace(content, delegate(Match match)
                {
                    return "<span class='courseCode' onclick='searchCourseCode(this)'>" + match.Value + "</span>";
                });
            }
            else
            {
                return null;
            }
        }
    }
}
