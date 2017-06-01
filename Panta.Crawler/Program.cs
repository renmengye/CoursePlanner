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

            UOfTCourses = new DefaultIIndexableCollection<Course>("University of Toronto", "uoft_courses", courseSigner,
                new UTEngCourseFetcher().FetchItems()
                //new UTArtsciCourseFetcher().FetchItems()
                //new UTSCCourseFetcher().FetchItems()
                .Concat<UTCourse>(new UTArtsciCourseFetcher().FetchItems())
                .Concat<UTCourse>(new UTSCCourseFetcher().FetchItems())
                .Concat<UTCourse>(new UTMCourseFetcher().FetchItems())
                );
            UOfTCourses.SaveBin();

            UOfTPrograms = new DefaultIIndexableCollection<SchoolProgram>("University of Toronto", "uoft_progs", progSigner,
                new UTArtsciProgramFetcher().FetchItems()
                .Concat<SchoolProgram>(new UTEngProgramFetcher(WebUrlConstants.EngPrograms).FetchItems())
                .Concat<SchoolProgram>(new UTSCProgramFetcher().FetchItems())
                .Concat<SchoolProgram>(new UTMProgramFetcher().FetchItems()));
            UOfTPrograms.SaveBin();
        }
    }
}
