using DDay.iCal;
using DDay.iCal.Serialization.iCalendar;
using Panta.DataModels;
using Panta.DataModels.Extensions.UT;
using Panta.Indexing;
using Panta.Indexing.Correctors;
using Panta.Searching;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Scheduler.Web.Handlers
{
    /// <summary>
    /// Summary description for ICalHandler
    /// </summary>
    public class ICalHandler : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            string courseStrings = Encoding.UTF8.GetString(Convert.FromBase64String(context.Request.QueryString["Courses"]));
            iCalendar calendar;

            if (this.TryParseCalendar(courseStrings, out calendar))
            {
                context.Response.ContentType = "text/calendar";
                context.Response.Headers["Content-Disposition"] = "attachment; filename=2018_2019_Calendar.ics";
                iCalendarSerializer serializer = new iCalendarSerializer(calendar);
                context.Response.Write(serializer.SerializeToString(calendar));
            }
            else
            {
                context.Response.ContentType = "text/plain";
                context.Response.Write("Unable to read the course IDs");
            }
        }

        public bool TryParseCalendar(string courseStrings, out iCalendar calendar)
        {
            calendar = new iCalendar();
            calendar.Scale = "GREGORIAN";
            calendar.Method = "PUBLISH";

            foreach (string courseString in courseStrings.Split(','))
            {
                uint[] courseIds;
                string sectionCode;
                if (!this.TryGetCourseIds(courseString, out courseIds)) return false;
                if (!this.TryGetSectionCode(courseString, out sectionCode)) return false;
                if (!TryAddCourse(courseIds[0], courseString, sectionCode, calendar))
                {
                    if (!TryAddCourse(courseIds[1], courseString, sectionCode, calendar))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public bool TryAddCourse(uint courseId, string courseString, string sectionCode, iCalendar calendar)
        {
            IIndexableCollection<Course> courses = (IIndexableCollection<Course>)HttpContext.Current.Application["Courses"];
            Course course;
            if (courses.TryGetItem(courseId, out course))
            {
                if (!courseString.StartsWith(course.Abbr)) return false;
                foreach (CourseSection section in course.Sections)
                {
                    if (section.Name == sectionCode)
                    {
                        UTCourseSection utSection = section as UTCourseSection;
                        UTCourse utCourse = course as UTCourse;
                        if (utSection != null && utCourse != null)
                        {
                            int order = 0;
                            foreach (CourseSectionTimeSpan timeSpan in utSection.ParsedTime.MeetTimes)
                            {
                                if (utCourse.SemesterDetail == "Fall" || utCourse.SemesterDetail == "Year")
                                {
                                    Event courseEvent = calendar.Create<Event>();

                                    if (utCourse.Faculty == "Engineering")
                                    {
                                        courseEvent.Start = new iCalDateTime(this.ConvertDateTime(new DateTime(2018, 9, 6), timeSpan.Day, timeSpan.Start), "America/Toronto");
                                        courseEvent.End = new iCalDateTime(this.ConvertDateTime(new DateTime(2018, 9, 6), timeSpan.Day, timeSpan.End), "America/Toronto");
                                        courseEvent.RecurrenceRules.Add(new RecurrencePattern(FrequencyType.Weekly)
                                        {
                                            //Count = 13
                                            Until = new DateTime(2018, 12, 5)
                                        });
                                    }
                                    else
                                    {
                                        courseEvent.Start = new iCalDateTime(this.ConvertDateTime(new DateTime(2018, 9, 6), timeSpan.Day, timeSpan.Start), "America/Toronto");
                                        courseEvent.End = new iCalDateTime(this.ConvertDateTime(new DateTime(2018, 9, 6), timeSpan.Day, timeSpan.End), "America/Toronto");
                                        courseEvent.RecurrenceRules.Add(new RecurrencePattern(FrequencyType.Weekly)
                                        {
                                            //Count = 13
                                            Until = new DateTime(2018, 12, 5)
                                        });
                                    }

                                    courseEvent.Summary = utCourse.Abbr + ": " + utCourse.Name + " " + utSection.Name;
                                    try
                                    {
                                        courseEvent.Location = utSection.Location.Split(' ')[order];
                                    }
                                    catch
                                    {
                                    }
                                }

                                if (utCourse.SemesterDetail == "Winter" || utCourse.SemesterDetail == "Year")
                                {
                                    Event courseEvent = calendar.Create<Event>();

                                    if (utCourse.Faculty == "Engineering")
                                    {
                                        courseEvent.Start = new iCalDateTime(this.ConvertDateTime(new DateTime(2019, 1, 7), timeSpan.Day, timeSpan.Start), "America/Toronto");
                                        courseEvent.End = new iCalDateTime(this.ConvertDateTime(new DateTime(2019, 1, 7), timeSpan.Day, timeSpan.End), "America/Toronto");

                                        courseEvent.RecurrenceRules.Add(new RecurrencePattern(FrequencyType.Weekly)
                                        {
                                            Until = new DateTime(2019, 4, 5)
                                            //Count = 13
                                        });
                                    }
                                    else
                                    {
                                        courseEvent.Start = new iCalDateTime(this.ConvertDateTime(new DateTime(2019, 1, 7), timeSpan.Day, timeSpan.Start), "America/Toronto");
                                        courseEvent.End = new iCalDateTime(this.ConvertDateTime(new DateTime(2019, 1, 7), timeSpan.Day, timeSpan.End), "America/Toronto");

                                        courseEvent.RecurrenceRules.Add(new RecurrencePattern(FrequencyType.Weekly)
                                        {
                                            Until = new DateTime(2019, 4, 5)
                                            //Count = 13
                                        });
                                    }
                                    courseEvent.Summary = utCourse.Abbr + ": " + utCourse.Name + " " + utSection.Name;
                                    try
                                    {
                                        courseEvent.Location = utSection.Location.Split(' ')[order];
                                    }
                                    catch
                                    {
                                    }
                                }
                                order++;
                            }
                        }
                        return true;
                    }
                }
            }
            return false;
        }

        private DateTime ConvertDateTime(DateTime offset, DayOfWeek day, byte quarters)
        {
            DateTime result = offset;
            while (result.DayOfWeek != day)
            {
                result = result.AddDays(1);
            }
            return result.AddHours(quarters / 4).AddMinutes(quarters % 4);
        }

        private bool TryGetCourseIds(string courseName, out uint[] courseId)
        {
            IIndexablePresenter<Course> presenter = (IIndexablePresenter<Course>)HttpContext.Current.Application["CoursePresenter"];
            try
            {
                courseId = presenter.GetIDMatches(courseName.Split(' ')[0], "code:", null).ToArray();
            }
            catch
            {
                courseId = new uint[] { };
                return false;
            }
            return true;
        }

        //private bool TryGetCourseId(string jsCourseId, out uint courseId)
        //{
        //    string number = String.Empty;
        //    foreach (char c in jsCourseId)
        //    {
        //        if (Char.IsNumber(c))
        //        {
        //            number += c;
        //        }
        //        else
        //        {
        //            break;
        //        }
        //    }
        //    return UInt32.TryParse(number, out courseId);
        //}

        private bool TryGetSectionCode(string courseString, out string sectionCode)
        {
            string[] parts = courseString.Split(' ');
            if (parts.Length < 2)
            {
                sectionCode = String.Empty;
                return false;
            }
            else
            {
                sectionCode = parts[1];
                return true;
            }
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}