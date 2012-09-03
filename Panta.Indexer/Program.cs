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
            Program p = new Program();
        }

        public Program()
        {
            School UOfT;
            if (File.Exists(UTSavePath))
            {
                UOfT = School.Read(UTSavePath);
                Index(UOfT);
            }
            else
            {
                throw new FileNotFoundException("Data file not found");
            }
        }

        public void Index(School school)
        {
            Indexer<Course> indexer = new Indexer<Course>(school.Abbr);
            indexer.Index(school.Courses.Select<KeyValuePair<uint, Course>, Course>(entry => entry.Value));
        }
    }
}
