using Panta.DataModels.Extensions.UT;
using Panta.Fetchers.Extensions.UT;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Panta.Fetchers.Extensions.UTSC
{
    public class UTSCCourseFetcher : IItemFetcher<UTCourse>
    {
        public IEnumerable<UTCourse> FetchItems()
        {
            IEnumerable<UTCourse> courseInfos = new UTSCCourseInfoHtmlFetcher(WebUrlConstants.UTSCTimetable).FetchItems();
            List<UTCourse> courseDetails = new List<UTCourse>();

            IItemFetcher<string> departmentFetcher = new UTSCDepartmentFetcher(WebUrlConstants.UTSCDepartment);
            Parallel.ForEach<string>(departmentFetcher.FetchItems(), new ParallelOptions() { MaxDegreeOfParallelism = Environment.ProcessorCount },delegate(string url)
            //foreach (var url in departmentFetcher.FetchItems())
            {
                IEnumerable<UTCourse> part = new UTSCCourseDetailFetcher(url).FetchItems();
                lock (this)
                {
                    courseDetails.AddRange(part);
                }
                });
            //}

            courseInfos = courseInfos.GroupJoin(courseDetails,
                (x => x.Abbr),
                (x => x.Abbr),
                ((x, y) => (CombineInfoDetail(x, y.FirstOrDefault()))),
                new UTCourseAbbrComparer());

            return courseInfos;
        }


        private UTCourse CombineInfoDetail(UTCourse info, UTCourse detail)
        {
            if (detail != null)
            {
                info.Name = detail.Name;
                info.Description = detail.Description;
                info.Corequisites = detail.Corequisites;
                info.Prerequisites = detail.Prerequisites;
                info.Exclusions = detail.Exclusions;
                info.DistributionRequirement = detail.DistributionRequirement;
                info.BreadthRequirement = detail.BreadthRequirement;
            }
            return info;
        }
    }
}
