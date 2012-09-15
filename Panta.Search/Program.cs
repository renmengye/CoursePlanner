using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Panta.Indexing;
using Panta.DataModels;
using Panta.Indexing.Expressions;
using Panta.Indexing.Correctors;
using Panta.Searching;
using Scheduler.Common;

namespace Panta.Search
{
    public class Program
    {
        public static void Main(string[] args)
        {
            string abbr = args.Length > 0 ? args[0] : "uoft";
            InvertedWordIndex index = InvertedWordIndex.Read(abbr + ".idx");
            School school = School.Read(abbr + ".bin");
            CourseSearchPresenter searchEngine = new CourseSearchPresenter(index, school);

            while (true)
            {
                Console.Write("Please input a query: ");
                string query = Console.ReadLine();

                //CourseSearchResult result = searchEngine.GetCourseList(query);

                //if (result.CodeNameMatches.Count > 0)
                //{
                //    Console.WriteLine("\nCourse:");
                //    Console.WriteLine(result.CodeNameMatches.First<Course>().ToString());
                //}
                //foreach (IIndexable item in result.RawMatches)
                //{
                //    Course course = item as Course;
                //    Console.WriteLine("{0}: {1}", course.Abbr, course.Name);
                //}
                //if (result.RawMatches.Count > 0) Console.WriteLine("\nRelevant:");
                //foreach (IIndexable item in result.RawMatches)
                //{
                //    Course course = item as Course;
                //    Console.WriteLine("{0}: {1}", course.Abbr, course.Name);
                //}
                //Console.WriteLine();
            }
        }
    }
}
