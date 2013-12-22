using Panta.DataModels;
using Panta.Searching;
using Scheduler.Common;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Http;

namespace Scheduler.Web.Controllers
{
    public class ProgramController : ApiController
    {
        private static Regex CodeRegex;

        static ProgramController()
        {
            CodeRegex = new Regex("(?<code>[A-Z]{3,4}[0-9]{2,3})(?<prefix>[HY][0-9])", RegexOptions.Compiled);
        }

        public SearchResult Get(string q)
        {
            if (!(bool)HttpContext.Current.Application["Updating"])
            {
                q = HttpUtility.HtmlDecode(q);
                IIndexablePresenter<SchoolProgram> presenter = (IIndexablePresenter<SchoolProgram>)HttpContext.Current.Application["ProgramPresenter"];
                return presenter.GetItemList(q);
            }
            return null;
        }

        public IEnumerable<SchoolProgram> Get(string q, bool raw)
        {
            if (!(bool)HttpContext.Current.Application["Updating"])
            {
                q = HttpUtility.HtmlDecode(q);
                ProgramSearchPresenter presenter = (ProgramSearchPresenter)HttpContext.Current.Application["ProgramPresenter"];

                return presenter.GetItemsFromIDs(presenter.GetIDMatches(q, "name:"));
            }
            return null;
        }

        public SchoolProgram Get(int id)
        {
            if (!(bool)HttpContext.Current.Application["Updating"])
            {
                IIndexablePresenter<SchoolProgram> presenter = (IIndexablePresenter<SchoolProgram>)HttpContext.Current.Application["ProgramPresenter"];
                IEnumerable<SchoolProgram> result = presenter.GetItemsFromIDs(presenter.GetIDMatches("id:" + id, null, null));
                SchoolProgram first = result.FirstOrDefault<SchoolProgram>();
                first.Description = first.Description.Replace("|", "<br />");
                first.Description = CodeRegex.Replace(first.Description, delegate(Match match)
                {
                    return "<span class='courseCode' onclick='searchCourseCode(this)'>" + match.Value + "</span>";
                });
                return first;
            }
            return null;
        }
    }
}
