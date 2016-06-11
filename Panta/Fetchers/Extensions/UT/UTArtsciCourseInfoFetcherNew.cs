using Newtonsoft.Json;
using Panta.DataModels;
using Panta.DataModels.Extensions.UT;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Panta.Fetchers.Extensions.UT
{
    /// <summary>
    /// New artsci course info fetcher. Data in JSON format provided by Colin 
    /// </summary>
    public class UTArtsciCourseInfoFetcherNew : TextFileItemFetcher<UTCourse>
    {
        public UTArtsciCourseInfoFetcherNew(string path) : base(path) { }

        public override IEnumerable<UTCourse> FetchItems()
        {
            dynamic allCourseData = JsonConvert.DeserializeObject(this.Content);
            List<UTCourse> allCourses = new List<UTCourse>();

            foreach (dynamic category in allCourseData.categorydata)
            //Parallel.ForEach<dynamic>((IEnumerable<dynamic>)allCourseData.categorydata,
            //    new ParallelOptions() { MaxDegreeOfParallelism = Environment.ProcessorCount }, 
            //    delegate(dynamic category)
            {
                foreach (dynamic course in category.course)
                {
                    UTCourse courseObj = new UTCourse();
                    string code = course.code;
                    courseObj.Code = code.Substring(0, 6);
                    string secstring = course.section;
                    if (code.Length > 8)
                    {
                        courseObj.Semester = code.Substring(9, 1);
                    }
                    else
                    {
                        courseObj.Semester = secstring.Substring(0, 1);
                    }
                    courseObj.SemesterPrefix = code.Substring(6, 2);
                    courseObj.Name = course.name;
                    courseObj.Campus = "UTSG";
                    Console.Out.Write("Course: " + courseObj.Abbr);
                    if (course.activities != null)
                    {
                        foreach (dynamic activity in course.activities)
                        {
                            UTCourseSection section = new UTCourseSection();
                            section.Name = activity.name;
                            section.Instructor = activity.instructor;
                            Console.Out.Write(" " + section.Name);
                            List<CourseSectionTimeSpan> meetTimes = new List<CourseSectionTimeSpan>();
                            bool hasTime = activity.times != null;
                            if (hasTime)
                            {
                                foreach (dynamic time in activity.times)
                                {
                                    string day = time.dayofweek;
                                    string timeFrom = time.time_from;
                                    string timeTo = time.time_to;
                                    CourseSectionTimeSpan timespan;
                                    timespan.Day = ((DayOfWeek)Enum.Parse(typeof(DayOfWeek), day));
                                    timespan.Start = (byte)(Convert.ToInt32(timeFrom.Substring(0, 2)) * 4 + Convert.ToInt32(timeFrom.Substring(3, 2)) / 15);
                                    timespan.End = (byte)(Convert.ToInt32(timeTo.Substring(0, 2)) * 4 + Convert.ToInt32(timeTo.Substring(3, 2)) / 15);
                                    meetTimes.Add(timespan);
                                }
                                section.ParsedTime = new CourseSectionTime(meetTimes);
                            }
                            else
                            {
                                section.Time = "TBA";
                            }
                            Console.Out.WriteLine(section.ParsedTime.ToString());
                            courseObj.Sections.Add(section);
                        }
                    }
                    Console.Out.WriteLine();
                    allCourses.Add(courseObj);
                }
            //});
            }
            Console.Out.WriteLine("Total number of courses: {0}", allCourses.Count);
            return allCourses;
        }
    }
}
