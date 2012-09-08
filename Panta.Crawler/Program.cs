using Panta.DataModels;
using Panta.DataModels.Extensions.UT;
using Panta.Fetchers;
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
            IItemFetcher<Course> engCourseFetcher = new UTEngCourseFetcher();

            //UOfT = new School("University of Toronto", "uoft", signer, artsciCourseFetcher.FetchItems());
            UOfT = new School("University of Toronto Engineering", "uofteng", signer, engCourseFetcher.FetchItems());
            UOfT.Save();
        }
    }
}
