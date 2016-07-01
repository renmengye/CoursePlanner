using Panta.DataModels;
using Panta.DataModels.Extensions.UT;
using Panta.Fetchers;
using Panta.Fetchers.Extensions.UT;
using Panta.Fetchers.Extensions.UTM;
using Panta.Fetchers.Extensions.UTSC;
using Panta.Indexing;
using System.Linq;
using System.Xml.Serialization;

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
            DefaultIIndexableCollection<Course> UOfTCourses;
            DefaultIIndexableCollection<SchoolProgram> UOfTPrograms;
            IdSigner<Course> courseSigner = new IdSigner<Course>();
            IdSigner<SchoolProgram> progSigner = new IdSigner<SchoolProgram>();

            IItemFetcher<UTCourse> artsciCourseFetcher = new UTArtsciCourseFetcher();
            IItemFetcher<UTCourse> artsciSeminarFetcher = new UTArtsciSeminarFetcher();
            IItemFetcher<UTCourse> engCourseFetcher = new UTEngCourseFetcher();
            IItemFetcher<UTCourse> utscCourseFetcher = new UTSCCourseFetcher();
            IItemFetcher<UTCourse> utmCourseFetcher = new UTMCourseFetcher();

            UOfTCourses = new DefaultIIndexableCollection<Course>("University of Toronto", "uoft_courses", courseSigner,
                engCourseFetcher.FetchItems()
                //artsciCourseFetcher.FetchItems()
                //utscCourseFetcher.FetchItems()
                .Concat<UTCourse>(artsciCourseFetcher.FetchItems())
                //.Concat<UTCourse>(artsciSeminarFetcher.FetchItems())
                .Concat<UTCourse>(utscCourseFetcher.FetchItems())
                .Concat<UTCourse>(utmCourseFetcher.FetchItems())
                );
            UOfTCourses.SaveBin();

            IItemFetcher<SchoolProgram> artsciProgramFetcher = new UTArtsciProgramFetcher();
            IItemFetcher<SchoolProgram> engProgramFetcher = new UTEngProgramFetcher(WebUrlConstants.EngPrograms);
            IItemFetcher<SchoolProgram> utscProgramFetcher = new UTSCProgramFetcher();
            IItemFetcher<SchoolProgram> utmProgramFetcher = new UTMProgramFetcher();

            UOfTPrograms = new DefaultIIndexableCollection<SchoolProgram>("University of Toronto", "uoft_progs", progSigner,
                artsciProgramFetcher.FetchItems()
                .Concat<SchoolProgram>(engProgramFetcher.FetchItems())
                .Concat<SchoolProgram>(utscProgramFetcher.FetchItems())
                .Concat<SchoolProgram>(utmProgramFetcher.FetchItems()));
            UOfTPrograms.SaveBin();
        }
    }
}
