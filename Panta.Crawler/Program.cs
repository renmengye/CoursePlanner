using Panta.DataModels;
using Panta.DataModels.Extensions.UT;
using Panta.Fetchers;
using Panta.Fetchers.Extensions.UT;
using Panta.Indexing;
using System.Linq;

namespace Panta
{
    public class Program
    {
        static void Main(string[] args)
        {
            Program p = new Program();
        }

        public Program()
        {
            School UOfT;
            IdSigner<Course> signer = new IdSigner<Course>();
            IItemFetcher<UTCourse> artsciCourseFetcher = new UTArtsciCourseFetcher();
            IItemFetcher<UTCourse> engCourseFetcher = new UTEngCourseFetcher();

            UOfT = new School("University of Toronto", "uoft", signer, artsciCourseFetcher.FetchItems().Concat<UTCourse>(engCourseFetcher.FetchItems()));
            //UOfT = new School("University of Toronto", "uoft", signer, engCourseFetcher.FetchItems());
            UOfT.Save();
        }
    }
}
