using Panta.DataModels;
using Panta.DataModels.Extensions.UT;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Panta.Fetchers.Extensions.UT
{
    public class UTArtsciCourseInfoFetcher : WebpageItemFetcher<UTCourse>
    {
        public UTArtsciCourseInfoFetcher(string url) : base(url) { }

        private static Regex AngleRegex;
        private static Regex CircleRegex;
        private static Regex CodeRegex;
        private static Regex CourseRegex;

        static UTArtsciCourseInfoFetcher()
        {
            AngleRegex = new Regex("<[^>]+>", RegexOptions.Compiled);
            CircleRegex = new Regex("[\u0020]*[\u0028][^\u0029]*[\u0029][\u0020]*", RegexOptions.Compiled);
            CodeRegex = new Regex("(?<code>[A-Z]{3}[0-9]{3})(?<prefix>[HY][0-9])", RegexOptions.Compiled);
            CourseRegex = new Regex(@"<tr>.+?</tr>", RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Compiled);
        }

        public override IEnumerable<UTCourse> FetchItems()
        {
            List<UTCourse> results = new List<UTCourse>();

            if (this.Content == null) return results;

            this.Content = this.Content.Replace("\r\n", String.Empty);

            MatchCollection matches = CourseRegex.Matches(this.Content);

            // Used to accumulating course meet times
            string tempTime = "";
            string tempLocation = "";

            foreach (Match match in matches)
            {
                string line = match.Value;
                line = line.Replace("</td>", "|");
                line = AngleRegex.Replace(line, String.Empty);
                line = CircleRegex.Replace(line, String.Empty);

                string[] properties = line.Split('|');

                Match codeMatch = CodeRegex.Match(properties[0]);

                // Get rid of those which the first course in the page does not start with legit course code
                if (!codeMatch.Success && results.Count == 0) continue;

                string code = codeMatch.Groups["code"].ToString();
                string prefix = codeMatch.Groups["prefix"].ToString();
                CourseSection courseSection = null;

                if (properties.Length < 4) continue;
                string semester = properties[1].Trim(' ');
                string name = properties[2].Trim(' ');
                string section = properties[3].Trim(' ');
                string wait = null;
                string time = null;
                string location = null;
                string instructor = null;
                string matchedLocation = null;


                // Meaning this section of this course is not cancelled
                if (properties.Length >= 8)
                {
                    wait = properties[4].Trim(' ');
                    time = properties[5].Replace(" ", "").Replace(",", "");
                    location = properties[6].Replace(" ", "");
                    instructor = properties[7].Trim(' ').Replace("&nbsp;", "");

                    if (wait.Contains("Cancel")) continue;

                    CourseSectionTime ptime;
                    if (UTCourseSectionTime.TryParseRawTime(time, out ptime))
                    {
                        // Construct same location n times matching the meeting times
                        if (!location.Equals("&nbsp;"))
                        {
                            matchedLocation = location;
                            for (int i = 1; i < ptime.MeetTimes.Count(); i++)
                            {
                                matchedLocation = String.Join(" ", matchedLocation, location);
                            }
                        }
                        else
                        {
                            // Get the location from the previous section
                            if (section.Equals("&nbsp;"))
                            {
                                if (results.Count > 0)
                                {
                                    if (results.Last<UTCourse>().Sections.Count > 0)
                                    {
                                        location = results.Last<UTCourse>().Sections.Last<CourseSection>().Location;
                                        location = location.Split(' ')[0];
                                        matchedLocation = location;
                                        for (int i = 1; i < ptime.MeetTimes.Count(); i++)
                                        {
                                            matchedLocation = String.Join(" ", matchedLocation, location);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                // For some exceptions, section time is not written in one string, need to accumulating meet times
                if (section.Equals("&nbsp;"))
                {
                    if (properties.Length >= 8)
                    {
                        tempTime += time;
                        tempLocation = String.Join(" ", tempLocation, matchedLocation);
                    }
                    else
                    {
                        tempTime = "";
                        tempLocation = "";
                    }
                }
                else
                {
                    if (properties.Length >= 8)
                    {
                        courseSection = new UTCourseSection()
                        {
                            Name = section,
                            WaitList = wait.Equals("Y"),
                            Instructor = instructor,
                            Time = time,
                            Location = location
                        };
                    }

                    UTCourse lastCourse = null;

                    if (results.Count > 0)
                    {
                        lastCourse = results.Last<UTCourse>();

                        if (!String.IsNullOrEmpty(tempTime))
                        {
                            if (lastCourse.Sections.Count > 0)
                            {
                                // Update the last tempTime and tempLocation
                                CourseSection lastSection = lastCourse.Sections.Last<CourseSection>();
                                lastSection.Time = tempTime;
                                lastSection.Location = tempLocation;
                            }
                        }
                    }

                    tempTime = time;
                    tempLocation = matchedLocation;

                    // Only need to add a section
                    if (!codeMatch.Success)
                    {
                        if (lastCourse != null)
                        {
                            if (courseSection != null)
                            {
                                lastCourse.Sections.Add(courseSection);
                                Console.Write(" {0} ", courseSection.Name);
                            }
                        }
                        continue;
                    }

                    // Construct a course
                    UTCourse course = new UTCourse()
                    {
                        Code = code,
                        Name = name,
                        Semester = semester,
                        SemesterPrefix = prefix,
                        Campus = "UTSG"
                    };

                    // Add the newly constructed courseSection (if not cancelled) into the course
                    if (courseSection != null)
                    {
                        course.Sections.Add(courseSection);
                        Console.Write("{0}Course: {1} {2}", Environment.NewLine, course.Abbr, courseSection.Name);
                    }
                    else
                    {
                        Console.Write("{0}Course: {1} ", Environment.NewLine, course.Abbr);
                    }
                    results.Add(course);
                }
            }

            if (results.Count > 0)
            {
                // Commit the last change to the last section
                if (results.Last<UTCourse>().Sections.Count > 0)
                {
                    CourseSection lastSec = results.Last<UTCourse>().Sections.Last<CourseSection>();
                    lastSec.Time = tempTime;
                    lastSec.Location = tempLocation;
                }
            }
            return results;
        }
    }
}
