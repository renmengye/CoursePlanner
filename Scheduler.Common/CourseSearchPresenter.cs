using Panta.DataModels;
using Panta.Indexing;
using Panta.Indexing.Correctors;
using Panta.Indexing.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scheduler.Common
{
    public class CourseSearchPresenter
    {
        private InvertedWordIndex Index { get; set; }
        private IIndexableCollection<Course> CourseCollection { get; set; }
        private ITermCorrector Corrector { get; set; }

        public CourseSearchPresenter(InvertedWordIndex index, IIndexableCollection<Course> collection)
        {
            this.Index = index;
            this.CourseCollection = collection;
            this.Corrector = new SuffixCorrector(this.Index.SortedKeys);
        }

        public HashSet<uint> GetCourseIDMatches(string query, string prefix = null)
        {
            if (prefix == null)
            {
                return SearchExpression.Parse(query, this.Corrector).Evaluate(this.Index);
            }
            else
            {
                return SearchExpression.ParseEachTermWithPrefix(query, prefix, this.Corrector).Evaluate(this.Index);
            }
        }

        public IEnumerable<Course> GetCoursesFromIDs(IEnumerable<uint> ids)
        {
            List<Course> result = new List<Course>();
            foreach (uint id in ids)
            {
                Course item;
                if (this.CourseCollection.TryGetItem(id, out item)) result.Add(item);
            }
            return result.OrderBy<Course, string>(x => x.Abbr);
        }

        private string FormatList(IEnumerable<Course> courses)
        {
            StringBuilder builder = new StringBuilder();
            foreach (Course course in courses)
            {
                builder.Append("<li id=c" + course.ID + " class='courseResult'>" + course.Abbr + ": " + course.Name + "</li>");
            }
            return builder.ToString();
        }

        /// <summary>
        /// Search a query and get a list of html-formatted results defined by CourseSearchResult
        /// </summary>
        /// <param name="query">Search query</param>
        /// <returns></returns>
        public CourseSearchResult GetCourseList(string query)
        {
            CourseSearchResult result = new CourseSearchResult();

            HashSet<uint> rawMatches = GetCourseIDMatches(query);
            HashSet<uint> codeMatches = GetCourseIDMatches(query, "code:");
            HashSet<uint> nameMatches = GetCourseIDMatches(query, "name:");

            rawMatches.ExceptWith(codeMatches);
            rawMatches.ExceptWith(nameMatches);
            nameMatches.ExceptWith(codeMatches);

            result.CodeNameMatches = FormatList(GetCoursesFromIDs(codeMatches).Concat<Course>(GetCoursesFromIDs(nameMatches)));

            HashSet<uint> desMatches, depMatches, preqMatches;
            result.DescriptionMatches = FormatList(GetCoursesFromIDs(desMatches = GetCourseIDMatches(query, "des:")));
            result.DepartmentMatches = FormatList(GetCoursesFromIDs(depMatches = GetCourseIDMatches(query, "dep:")));
            result.PrerequisiteMatches = FormatList(GetCoursesFromIDs(preqMatches = GetCourseIDMatches(query, "preq:")));

            rawMatches.ExceptWith(desMatches);
            rawMatches.ExceptWith(depMatches);
            rawMatches.ExceptWith(preqMatches);
            result.RawMatches = FormatList(GetCoursesFromIDs(rawMatches));

            return result;
        }
    }
}
