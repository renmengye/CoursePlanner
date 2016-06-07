using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System.IO;
using System.Diagnostics;
using Panta.DataModels.Extensions.UT;
using Panta.DataModels;
using System.Collections.Generic;

namespace Panta.Tests
{
    [TestClass]
    public class JsonTest
    {
        [TestMethod]
        public void TestMethod1()
        {
            string content = File.ReadAllText("C:\\Users\\renme_000\\Downloads\\run_results.json");
            dynamic allCourseData = JsonConvert.DeserializeObject(content);
            List<UTCourse> allCourses = new List<UTCourse>();
            foreach (dynamic category in allCourseData.categorydata)
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
                    Trace.Write(courseObj.Abbr);
                    try
                    {
                        foreach (dynamic activity in course.activities)
                        {
                            CourseSection section = new CourseSection();
                            section.Name = activity.name;
                            Trace.Write(" " + section.Name);
                            List<CourseSectionTimeSpan> meetTimes = new List<CourseSectionTimeSpan>();
                            foreach (dynamic time in activity.times)
                            {
                                string day = time.dayofweek;
                                string timeFrom = time.time_from;
                                string timeTo = time.time_to;
                                CourseSectionTimeSpan timespan;
                                timespan.Day = ((DayOfWeek)Enum.Parse(typeof(DayOfWeek), day));
                                timespan.Start = (byte)(Convert.ToInt32(timeFrom.Substring(0, 2)) * 4 + Convert.ToInt32(timeFrom.Substring(3, 2)));
                                timespan.End = (byte)(Convert.ToInt32(timeTo.Substring(0, 2)) * 4 + Convert.ToInt32(timeTo.Substring(3, 2)));
                                meetTimes.Add(timespan);
                            }
                            section.Time = new CourseSectionTime(meetTimes).ToString();
                            courseObj.Sections.Add(section);
                        }
                    }
                    catch (NullReferenceException e)
                    {
                        continue;
                    }
                    Trace.WriteLine("");
                    allCourses.Add(courseObj);
                }
            }
            Trace.WriteLine(string.Format("Total number of courses: {0}", allCourses.Count));
        }
    }
}
