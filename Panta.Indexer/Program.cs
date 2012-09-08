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
        public const string UTSavePath = "uoft.bin";
        
        public static void Main(string[] args)
        {
            School school;
            string path = args.Length > 0 ? args[0] : UTSavePath;
            if (File.Exists(path))
            {
                school = School.Read(path);
                Index(school);
            }
            else
            {
                throw new FileNotFoundException("Data file not found");
            }
        }

        public static void Index(School school)
        {
            Indexer<Course> indexer = new Indexer<Course>(school.Abbr);
            indexer.Index(school.Courses);
        }
    }
}
