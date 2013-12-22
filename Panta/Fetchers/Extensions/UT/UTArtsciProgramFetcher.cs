using Panta.DataModels;
using Panta.DataModels.Extensions.UT;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Panta.Fetchers.Extensions.UT
{
    public class UTArtsciProgramFetcher : IItemFetcher<SchoolProgram>
    {
        public IEnumerable<SchoolProgram> FetchItems()
        {
            List<SchoolProgram> results = new List<SchoolProgram>();

            IItemFetcher<UTDepartment> depFetcher = new UTArtsciDepartmentFetcher();

            // Parallel threads for fetching each department
            Parallel.ForEach<UTDepartment>(depFetcher.FetchItems(), new ParallelOptions() { MaxDegreeOfParallelism = Environment.ProcessorCount }, delegate(UTDepartment dep)
            //foreach (UTDepartment dep in depFetcher.FetchItems())
            {
                IItemFetcher<SchoolProgram> programFetcher = new UTArtsciProgramDetailFetcher(dep.DetailUrl);
                lock (this)
                {
                    results.AddRange(programFetcher.FetchItems());
                }
            });

            // Acturial science
            //IItemFetcher<SchoolProgram> actProgramFetcher = new UTArtsciProgramDetailFetcher("http://www.artsandscience.utoronto.ca/ofr/calendar/crs_act.htm");
            //results.AddRange(actProgramFetcher.FetchItems());

            return results;
        }
    }
}
