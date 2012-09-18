using Panta.DataModels;
using Panta.Indexing;
using Panta.Indexing.Correctors;
using Panta.Searching;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Scheduler.Common
{
    public class ProgramSearchPresenter : DefaultIIndexablePresenter<SchoolProgram>
    {
        public ProgramSearchPresenter(InvertedWordIndex index, IIndexableCollection<SchoolProgram> collection) : base(index, collection) { }

        public override IEnumerable<SchoolProgram> GetItemsFromIDs(IEnumerable<uint> ids)
        {
            return base.GetItemsFromIDs(ids).OrderBy<SchoolProgram, string>(x => x.Name); ;
        }

        public override string FormatList(IEnumerable<SchoolProgram> programs)
        {
            StringBuilder builder = new StringBuilder();
            foreach (SchoolProgram program in programs)
            {
                builder.Append("<li id='p" + program.ID + "' class='programResult'>" + program.Name + "</li>");
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
            SearchResult result = new SearchResult();
            result.Matches = this.FormatList(this.GetItemsFromIDs(this.GetIDMatches(query, "name:")));
            return result;
        }
    }
}
