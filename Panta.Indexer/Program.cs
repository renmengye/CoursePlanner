using Panta.DataModels;
using Panta.Indexing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
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

            if (File.Exists(cpath) && File.Exists(ppath))
            {
                school = DefaultIIndexableCollection<Course>.ReadBin(cpath);
                pschool = DefaultIIndexableCollection<SchoolProgram>.ReadBin(ppath);
                Task indexTask = Task.Run(() =>
                {
                    Index<Course>(school);
                    Index<SchoolProgram>(pschool);
                });
                while (indexTask.Status == TaskStatus.Running)
                {
                    Console.WriteLine("Running...");
                    Thread.Sleep(1000);
                }
                indexTask.Wait();
            }
            else
            {
                throw new FileNotFoundException("Data file not found");
            }

            Console.WriteLine("Finished indexing");
        }

        public static void Index<T>(DefaultIIndexableCollection<T> school) where T : IIndexable
        {
            Indexer<T> indexer = new Indexer<T>(school.Abbr);
            indexer.Index(school.Items);
        }
    }
}
