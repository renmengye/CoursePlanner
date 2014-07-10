using Panta.DataModels;
using Panta.DataModels.Extensions.UT;
using Panta.Fetchers.Extensions.UTM;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Panta.Fetchers.Extensions.UTSC
{
    /// <summary>
    /// This calls the UTSU api to fetch the csv instead of the webpage to parse
    /// </summary>
    public class UTSCCourseInfoFetcher : WebpageItemFetcher<UTCourse>
    {
        private static Regex CodeRegex, CsvRegex;

        static UTSCCourseInfoFetcher()
        {
            CodeRegex = new Regex("(?<code>[A-Z]{4}[0-9]{2})(?<prefix>[HY][0-9]) (?<semester>[FSY])", RegexOptions.Compiled);
            CsvRegex = new Regex("\"(?<content>.*?)\"", RegexOptions.Compiled);
        }

        /// <summary>
        /// Instantiate an course info fecther with a department abbreviation
        /// </summary>
        /// <param name="abbr"></param>
        public UTSCCourseInfoFetcher(string url) : base(url) { }

        public override IEnumerable<UTCourse> FetchItems()
        {
            List<UTCourse> results = new List<UTCourse>();

            int lineCount = 0;
            // Parse the csv file by line
            foreach (string line in this.Content.TrimStart('\n').Split('\n'))
            {
                lineCount++;
                if (lineCount == 1) continue;
                int columnCount = 0;

                UTCourse course = new UTCourse()
                {
                    Campus = "UTSC"
                };
                UTCourseSection courseSection = new UTCourseSection();
                CourseSectionTimeSpan span = new CourseSectionTimeSpan();

                MatchCollection lineMatches = CsvRegex.Matches(line);
                foreach (Match column in lineMatches)
                {
                    string content = column.Value.Trim('\"');
                    switch (columnCount)
                    {
                        // Course code
                        case (0):
                            {
                                Match codeMatch = CodeRegex.Match(content);
                                course.Code = codeMatch.Groups["code"].Value;
                                course.SemesterPrefix = codeMatch.Groups["prefix"].Value;
                                course.Semester = codeMatch.Groups["semester"].Value;
                                break;
                            }
                        // Section name
                        case (1):
                            {
                                courseSection.Name = content;
                                break;
                            }
                        // Day
                        case (3):
                            {
                                span.Day = UTMTimeParser.ParseDay(content);
                                break;
                            }
                        // Start time
                        case (4):
                            {
                                byte start;
                                UTCourseSectionTimeSpan.TryParseTimeSpanInt(content, out start);
                                span.Start = start;
                                break;
                            }
                        // End time
                        case (5):
                            {
                                byte end;
                                UTCourseSectionTimeSpan.TryParseTimeSpanInt(content, out end);
                                span.End = end;
                                break;
                            }
                        // Location
                        case (6):
                            {
                                courseSection.Location = content.Replace(" ", String.Empty);
                                break;
                            }
                        // Instructor
                        case (7):
                            {
                                courseSection.Instructor = content;
                                break;
                            }
                        default:
                            {
                                break;
                            }
                    }
                    columnCount++;
                }
                if (String.IsNullOrEmpty(course.Abbr)) continue;

                // Check if it is the same course
                if (results.Count > 0)
                {
                    UTCourse lastCourse = results.Last();
                    if (lastCourse.Abbr == course.Abbr)
                    {
                        UTCourseSection lastSection = lastCourse.Sections.Last() as UTCourseSection;
                        // Check if it is the same section
                        if (lastSection.Name == courseSection.Name)
                        {
                            // Add the meet time
                            List<CourseSectionTimeSpan> meets = lastSection.ParsedTime.MeetTimes.ToList();
                            meets.Add(span);
                            lastSection.ParsedTime = new CourseSectionTime(meets);
                            String.Join(" ", lastSection.Location, courseSection.Location);
                            continue;
                        }
                        else
                        {
                            // Add the course section
                            List<CourseSectionTimeSpan> meets = new List<CourseSectionTimeSpan>();
                            meets.Add(span);
                            courseSection.ParsedTime = new CourseSectionTime(meets);
                            lastCourse.Sections.Add(courseSection);
                            continue;
                        }
                    }
                }

                // Add the course
                List<CourseSectionTimeSpan> meets2 = new List<CourseSectionTimeSpan>();
                meets2.Add(span);
                courseSection.ParsedTime = new CourseSectionTime(meets2);

                course.Sections.Add(courseSection);
                results.Add(course);
            }

            return results;
        }
    }
}
