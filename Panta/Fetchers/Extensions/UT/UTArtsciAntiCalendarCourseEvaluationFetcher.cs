//using iTextSharp.text.pdf;
//using iTextSharp.text.pdf.parser;
using Panta.DataModels.Extensions.UT;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Panta.Fetchers.Extensions.UT
{
    public class UTArtsciAntiCalendarCourseEvaluationFetcher
    {
        public string Content { get; set; }

        private static Regex CourseRegex;

        static UTArtsciAntiCalendarCourseEvaluationFetcher()
        {
            CourseRegex =
                new Regex("",
                RegexOptions.Multiline | RegexOptions.Compiled);
        }

        public UTArtsciAntiCalendarCourseEvaluationFetcher(string url)
        {
            //PdfReader reader = new PdfReader(url);
            //this.Content = String.Empty;
            //for (int page = 1; page <= reader.NumberOfPages; page++)
            //{
            //    this.Content += PdfTextExtractor.GetTextFromPage(reader, page);
            //}
            //reader.Close();
        }

        public Dictionary<string, UTCourseEvaluation> FetchItems()
        {
            Dictionary<string, UTCourseEvaluation> result = new Dictionary<string, UTCourseEvaluation>();

            return null;
        }
    }
}
