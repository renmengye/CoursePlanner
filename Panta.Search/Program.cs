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
            InvertedWordIndex index = InvertedWordIndex.Read("uoft.idx");
            School school = School.Read("uoft.bin");
            while (true)
            {
                Console.Write("Please input a query: ");
                string query = Console.ReadLine();

                IExpression expression = SearchExpression.Parse(query, new SuffixCorrector(index.SortedKeys.ToArray<string>()));

                Console.WriteLine("Search results: ");
                foreach (uint id in expression.Evaluate(index))
                {
                    Course course;
                    if (school.Courses.TryGetValue(id, out course))
                    {
                        Console.WriteLine(course.Code);
                    }
                }
            }
        }
    }
}
