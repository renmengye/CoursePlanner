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
            return base.GetItemsFromIDs(ids).OrderBy<Course, string>(x => x.Abbr); ;
        }

        public override string FormatList(IEnumerable<Course> courses)
        {
            StringBuilder builder = new StringBuilder();
            foreach (Course course in courses)
            {
                builder.Append("<li id='c" + course.ID + "' class='courseResult'>" + course.Abbr + ": "
                    + (course.Name != null ? ((course.Name.Length > 30 ? course.Name.Substring(0, 30) + "..." : course.Name)) : "") + "</li>");
            }
            return builder.ToString();
        }

        public IEnumerable<SimpleCourse> GetRawList(string query)
        {
            List<Course> result = new List<Course>();
            HashSet<uint> rawMatches = GetIDMatches(query, null);
            HashSet<uint> codeMatches = GetIDMatches(query, "code:");
            HashSet<uint> nameMatches = GetIDMatches(query, "name:");

            if (codeMatches.Count == 0 && nameMatches.Count == 0)
            {
                codeMatches.UnionWith(rawMatches);
            }

            rawMatches.ExceptWith(codeMatches);
            rawMatches.ExceptWith(nameMatches);
            nameMatches.ExceptWith(codeMatches);

            result.AddRange(GetItemsFromIDs(codeMatches)
                .Concat<Course>(GetItemsFromIDs(nameMatches)));

            if (codeMatches.Count < 5)
            {
                HashSet<uint> desMatches, depMatches, preqMatches, postMatches;
                result.AddRange(GetItemsFromIDs(desMatches = GetIDMatches(query, "des:")));
                result.AddRange(GetItemsFromIDs(depMatches = GetIDMatches(query, "dep:")));
                result.AddRange(GetItemsFromIDs(preqMatches = GetIDMatches(query, "preq:")));
                result.AddRange(GetItemsFromIDs(postMatches = GetIDMatches(query, "post:")));

                rawMatches.ExceptWith(desMatches);
                rawMatches.ExceptWith(depMatches);
                rawMatches.ExceptWith(preqMatches);
                rawMatches.ExceptWith(postMatches);

                result.AddRange(GetItemsFromIDs(rawMatches));
            }
            else
            {
                result = result.Take(20).ToList();
            }
            return result.Select(x => new SimpleCourse()
            {
                Id = x.ID,
                Abbr = x.Abbr,
                Name = x.Name
            });
        }

        /// <summary>
        /// Search a query and get a list of html-formatted results defined by CourseSearchResult
        /// </summary>
        /// <param name="query">Search query</param>
        /// <returns></returns>
        public override SearchResult GetItemList(string query)
        {
            CourseSearchResult result = new CourseSearchResult();

            HashSet<uint> rawMatches = GetIDMatches(query, null);
            HashSet<uint> codeMatches = GetIDMatches(query, "code:");
            HashSet<uint> nameMatches = GetIDMatches(query, "name:");

            if (codeMatches.Count == 0 && nameMatches.Count == 0)
            {
                codeMatches.UnionWith(rawMatches);
            }

            rawMatches.ExceptWith(codeMatches);
            rawMatches.ExceptWith(nameMatches);
            nameMatches.ExceptWith(codeMatches);
                        
            if (codeMatches.Count < 5)
            {
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
            }
            result.CodeNameMatches = FormatList(GetItemsFromIDs(codeMatches.Concat(nameMatches).Take(20)));

            return result;
        }
    }
}
