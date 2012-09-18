using Panta.DataModels;
using Panta.Indexing;
using Panta.Indexing.Correctors;
using Panta.Indexing.Expressions;
using Panta.Searching;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scheduler.Common
{
    public class CourseSearchPresenter : DefaultIIndexablePresenter<Course>
    {
        public CourseSearchPresenter(InvertedWordIndex index, IIndexableCollection<Course> collection) : base(index, collection) { }

        public override IEnumerable<Course> GetItemsFromIDs(IEnumerable<uint> ids)
        {
            return base.GetItemsFromIDs(ids).OrderBy<Course, string>(x => x.Abbr);;
        }

        public override string FormatList(IEnumerable<Course> courses)
        {
            StringBuilder builder = new StringBuilder();
            foreach (Course course in courses)
            {
                builder.Append("<li id='c" + course.ID + "' class='courseResult'>" + course.Abbr + ": " + course.Name + "</li>");
            }
            return builder.ToString();
        }

        /// <summary>
        /// Search a query and get a list of html-formatted results defined by CourseSearchResult
        /// </summary>
        /// <param name="query">Search query</param>
        /// <returns></returns>
        public override SearchResult GetItemList(string query)
        {
            CourseSearchResult result = new CourseSearchResult();

            HashSet<uint> rawMatches = GetIDMatches(query);
            HashSet<uint> codeMatches = GetIDMatches(query, "code:");
            HashSet<uint> nameMatches = GetIDMatches(query, "name:");

            rawMatches.ExceptWith(codeMatches);
            rawMatches.ExceptWith(nameMatches);
            nameMatches.ExceptWith(codeMatches);

            result.CodeNameMatches = FormatList(GetItemsFromIDs(codeMatches).Concat<Course>(GetItemsFromIDs(nameMatches)));

            HashSet<uint> desMatches, depMatches, preqMatches, postMatches;
            result.DescriptionMatches = FormatList(GetItemsFromIDs(desMatches = GetIDMatches(query, "des:")));
            result.DepartmentMatches = FormatList(GetItemsFromIDs(depMatches = GetIDMatches(query, "dep:")));
            result.PrerequisiteMatches = FormatList(GetItemsFromIDs(preqMatches = GetIDMatches(query, "preq:")));
            result.PostrequisiteMatches = FormatList(GetItemsFromIDs(postMatches = GetIDMatches(query, "post:")));

            rawMatches.ExceptWith(desMatches);
            rawMatches.ExceptWith(depMatches);
            rawMatches.ExceptWith(preqMatches);
            rawMatches.ExceptWith(postMatches);
            result.RawMatches = FormatList(GetItemsFromIDs(rawMatches));

            return result;
        }
    }
}
