using Panta.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Panta.Fetchers.Extensions.UTM
{
    public class UTMProgramFetcher : IItemFetcher<SchoolProgram>
    {
        public IEnumerable<SchoolProgram> FetchItems()
        {
            List<SchoolProgram> results = new List<SchoolProgram>();
            IEnumerable<string> urls = new UTMProgramUrlFetcher("https://registrar.utm.utoronto.ca/student/calendar/program_list.pl").FetchItems();
            Parallel.ForEach<string>(urls, new ParallelOptions() { MaxDegreeOfParallelism = Environment.ProcessorCount },
                delegate(string url)
                {
                    IEnumerable<SchoolProgram> part = new UTMProgramDetailFetcher(url).FetchItems();
                    lock (this)
                    {
                        results.AddRange(part);
                    }
                });

            return results;
        }
    }
}
