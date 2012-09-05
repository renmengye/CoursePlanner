using Panta.Fetchers;

namespace Panta.DataModels.Extensions.UT
{
    public class UTDepartment : IName
    {
        public string Name { get; set; }
        public string Abbr { get; set; }
        public string Url { get; set; }
        public string DetailUrl { get; set; }
    }
}
