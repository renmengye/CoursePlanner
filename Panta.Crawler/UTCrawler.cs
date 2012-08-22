using System.IO;
using System.Linq;
using Panta.DataModels;
using Panta.Formatters;
using Panta.Indexing;
using System.Collections.Generic;

namespace Panta
{
    public class UTCrawler
    {
        public const string UTSavePath = "uoft.bin";

        static void Main(string[] args)
        {
            UTCrawler p = new UTCrawler();
        }

        public UTCrawler()
        {
            School UOfT;
            if (File.Exists(UTSavePath))
            {
                UOfT = School.Read(UTSavePath);
                Index(UOfT);
            }
            else
            {
                UOfT = new School("University of Toronto", "uoft", new UTDepartmentFormatter());
                UOfT.FetchDepartments();
                UOfT.Save();
            }
        }

        public void Index(School school)
        {
            Indexer<Course> indexer = new Indexer<Course>(school.Abbr);
            indexer.Index(school.Courses.Select<KeyValuePair<uint, Course>, Course>(entry => entry.Value));
        }
    }
}
