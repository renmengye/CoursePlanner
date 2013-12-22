using Panta.DataModels;
using Panta.Indexing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Panta.Indexer
{
    public class Program
    {
        public const string UTCoursesSavePath = "uoft_courses.bin";
        public const string UTProgramsSavePath = "uoft_progs.bin";

        public static void Main(string[] args)
        {
            DefaultIIndexableCollection<Course> school;
            DefaultIIndexableCollection<SchoolProgram> pschool;
            string cpath = args.Length > 0 ? args[0] : UTCoursesSavePath;
            string ppath = args.Length > 1 ? args[1] : UTProgramsSavePath;

            if (File.Exists(cpath))
            {
                school = DefaultIIndexableCollection<Course>.ReadBin(cpath);
                Index<Course>(school);
            }
            else
            {
                throw new FileNotFoundException("Data file not found");
            }

            if (File.Exists(ppath))
            {
                pschool = DefaultIIndexableCollection<SchoolProgram>.ReadBin(ppath);
                Index<SchoolProgram>(pschool);
            }
            else
            {
                throw new FileNotFoundException("Data file not found");
            }
        }

        public static void Index<T>(DefaultIIndexableCollection<T> school) where T : IIndexable
        {
            Indexer<T> indexer = new Indexer<T>(school.Abbr);
            indexer.Index(school.Items);
        }
    }
}
