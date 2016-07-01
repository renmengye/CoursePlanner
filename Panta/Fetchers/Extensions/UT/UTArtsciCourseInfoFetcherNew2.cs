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
    public class UTArtsciCourseInfoFetcherNew2 : WebpageItemFetcher<UTCourse>
    {
        public UTArtsciCourseInfoFetcherNew2(string path) : base(path) { }

        public override IEnumerable<UTCourse> FetchItems()
        {
            dynamic allCourseData = JsonConvert.DeserializeObject(this.Content);
            List<UTCourse> allCourses = new List<UTCourse>();

            foreach (dynamic coursepair in allCourseData)
            //Parallel.ForEach<dynamic>((IEnumerable<dynamic>)allCourseData.categorydata,
            //    new ParallelOptions() { MaxDegreeOfParallelism = Environment.ProcessorCount }, 
            //    delegate(dynamic category)
            {
                //dynamic course = pair.Value;
                dynamic course = coursepair.Value;
                //foreach (dynamic course in category.course)
                //{
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
                courseObj.Name = course.courseTitle;
                courseObj.Campus = "UTSG";
                Console.Out.Write("Course: " + courseObj.Abbr);
                if (course.meetings != null)
                {
                    //foreach (dynamic activity in course.activities)
                    
                    foreach (dynamic meeting in course.meetings)
                    {
                        dynamic activity = meeting.Value;
                        UTCourseSection section = new UTCourseSection();
                        //section.Name = activity.name;
                        section.Name = (string)activity.teachingMethod + (string)activity.sectionNumber;
                        //section.Instructor = activity.instructor;

                        List<string> instructors = new List<string>();
                        //Dictionary<string, string> instructorTable = new Dictionary<string, string>();
                        foreach (dynamic instructorpair in activity.instructors)
                        {
                            instructors.Add((string)instructorpair.Value.firstName + " " + (string)instructorpair.Value.lastName);
                        }
                        section.Instructor = string.Join(", ", instructors);
                        Console.Out.Write(" " + section.Name);
                        List<CourseSectionTimeSpan> meetTimes = new List<CourseSectionTimeSpan>();
                        bool hasTime = activity.schedule != null;
                        if (hasTime)
                        {
                            foreach (dynamic timepair in activity.schedule)
                            {
                                dynamic time = timepair.Value;
                                string day = time.meetingDay;
                                if (day != null)
                                {
                                    string timeFrom = time.meetingStartTime;
                                    string timeTo = time.meetingEndTime;
                                    CourseSectionTimeSpan timespan;
                                    switch (day)
                                    {
                                        case "MO":
                                            {
                                                timespan.Day = DayOfWeek.Monday;
                                                break;
                                            }
                                        case "TU":
                                            {
                                                timespan.Day = DayOfWeek.Tuesday;
                                                break;
                                            }
                                        case "WE":
                                            {
                                                timespan.Day = DayOfWeek.Wednesday;
                                                break;
                                            }
                                        case "TH":
                                            {
                                                timespan.Day = DayOfWeek.Thursday;
                                                break;
                                            }
                                        case "FR":
                                            {
                                                timespan.Day = DayOfWeek.Friday;
                                                break;
                                            }
                                        default:
                                            {
                                                throw new Exception("Unknown day: " + day);
                                            }
                                    }
                                    //timespan.Day = ((DayOfWeek)Enum.Parse(typeof(DayOfWeek), day));
                                    timespan.Start = (byte)(Convert.ToInt32(timeFrom.Substring(0, 2)) * 4 + Convert.ToInt32(timeFrom.Substring(3, 2)) / 15);
                                    timespan.End = (byte)(Convert.ToInt32(timeTo.Substring(0, 2)) * 4 + Convert.ToInt32(timeTo.Substring(3, 2)) / 15);
                                    meetTimes.Add(timespan);
                                }
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
                    //}
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
