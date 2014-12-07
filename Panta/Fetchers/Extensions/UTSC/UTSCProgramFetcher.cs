using Panta.DataModels;
using Panta.Fetchers.Extensions.UT;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Panta.Fetchers.Extensions.UTSC
{
    public class UTSCProgramFetcher : IItemFetcher<SchoolProgram>
    {
        public IEnumerable<SchoolProgram> FetchItems()
        {
            List<SchoolProgram> results = new List<SchoolProgram>();

            IItemFetcher<string> departmentFetcher = new UTSCDepartmentFetcher(WebUrlConstants.UTSCDepartment);
            Parallel.ForEach<string>(departmentFetcher.FetchItems(), new ParallelOptions() { MaxDegreeOfParallelism = Environment.ProcessorCount }, delegate(string url)
            //foreach (var url in departmentFetcher.FetchItems())
            {
                IEnumerable<SchoolProgram> part = new UTSCProgramDetailFetcher(url).FetchItems();
                lock (this)
                {
                    results.AddRange(part);
                }
            });
            //}

            return results;
        }
    }
}
