using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Panta.Indexing;
using Panta.DataModels;
using Panta.Indexing.Expressions;
using Panta.Indexing.Correctors;

namespace Panta.Search
{
    public class Program
    {
        public static void Main(string[] args)
        {
            string abbr = args.Length > 0 ? args[0] : "uoft";
            InvertedWordIndex index = InvertedWordIndex.Read(abbr + ".idx");
            School school = School.Read(abbr + ".bin");
            while (true)
            {
                Console.Write("Please input a query: ");
                string query = Console.ReadLine();

                IExpression expression = SearchExpression.Parse(query, new SuffixCorrector(index.SortedKeys));

                Console.WriteLine("Search results: ");
                foreach (uint id in expression.Evaluate(index))
                {
                    Course course;
                    if (school.TryGetCourse(id, out course))
                    {
                        Console.WriteLine("{0}: {1}", course.Abbr, course.Name);
                    }
                }
            }
        }
    }
}
