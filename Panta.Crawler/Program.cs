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
            DefaultIIndexableCollection<Course> UOfTCourses;
            DefaultIIndexableCollection<SchoolProgram> UOfTPrograms;
            IdSigner<Course> courseSigner = new IdSigner<Course>();
            IdSigner<SchoolProgram> progSigner = new IdSigner<SchoolProgram>();
            
            IItemFetcher<UTCourse> artsciCourseFetcher = new UTArtsciCourseFetcher();
            IItemFetcher<UTCourse> engCourseFetcher = new UTEngCourseFetcher();

            UOfTCourses = new DefaultIIndexableCollection<Course>("University of Toronto", "uoft_courses", courseSigner, artsciCourseFetcher.FetchItems().Concat<UTCourse>(engCourseFetcher.FetchItems()));
            UOfTCourses.Save();


            IItemFetcher<SchoolProgram> artsciProgramFetcher = new UTArtsciProgramFetcher();
            UOfTPrograms = new DefaultIIndexableCollection<SchoolProgram>("University of Toronto", "uoft_progs", progSigner, artsciProgramFetcher.FetchItems());
            UOfTPrograms.Save();
        }
    }
}
