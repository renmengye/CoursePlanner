using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Panta.Indexing;

namespace Panta.DataModels
{
    [Serializable]
    public class CourseSection
    {
        private string _time;
        // Contains time, location, waitlist, instructor
        public string Time
        {
            get
            {
                return this._time;
            }
            set
            {
                this._time = value;
                this.DetailTime = ParseRawTime(value);
            }
        }
        public string DetailTime { get; private set; }

        private string _name;
        public string Name
        {
            get
            {
                return this._name;
            }
            set
            {
                this._name = value;
                this.IsLecture = value[0] == 'L';
            }
        }
        public bool IsLecture { get; private set; }
        public string Location { get; set; }
        public bool WaitList { get; set; }
        public string Instructor { get; set; }

        /// <summary>
        /// Parse the time from raw form M2-4 or M2 to Monday 2pm-4pm 2hours
        /// </summary>
        /// <param name="raw">Raw form M2-4 or M2</param>
        /// <returns>Full form Monday 2pm-4pm 2hours</returns>
        private static string ParseRawTime(string raw)
        {
            if (String.IsNullOrEmpty(raw)) return String.Empty;
            string weekday = String.Empty;
            int startTime;
            int endTime;
            switch (raw[0])
            {
                case 'M':
                    weekday = "Monday";
                    break;
                case 'T':
                    weekday = "Tuesday";
                    break;
                case 'W':
                    weekday = "Wednesday";
                    break;
                case 'R':
                    weekday = "Thursday";
                    break;
                case 'F':
                    weekday = "Friday";
                    break;
            }
            string[] time = raw.Substring(1).Split('-');
            Int32.TryParse(time[0], out startTime);
            if (time.Length > 1)
            {
                Int32.TryParse(time[1], out endTime);
            }
            else
            {
                if (startTime == 12) { endTime = 1; } else { endTime = startTime + 1; }
            }
            string startTimeString = ToTimeString(startTime);
            string endTimeString = ToTimeString(endTime);
            string durationString = (endTime - startTime).ToString() + "hr";
            return String.Join(" ", weekday, startTimeString, endTimeString, durationString);
        }

        private static string ToTimeString(int time)
        {
            if (time > 7)
            {
                return time.ToString() + "am";
            }
            else
            {
                return time.ToString() + "pm";
            }
        }

        /// <summary>
        /// Put all the properties into a form with prefix and root
        /// </summary>
        /// <returns>A list of prefixes and roots</returns>
        protected IEnumerable<IndexString> GetIndexStrings()
        {
            List<IndexString> strings = new List<IndexString>();

            // Only index the time if the section is a lecture section
            if (!String.IsNullOrEmpty(this.Time) && this.IsLecture) strings.Add(new IndexString("time:", this.Time));
            if (!String.IsNullOrEmpty(this.DetailTime) && this.IsLecture) strings.Add(new IndexString("dtime:", this.DetailTime));
            if (!String.IsNullOrEmpty(this.Name)) strings.Add(new IndexString("name:", this.Name));
            if (!String.IsNullOrEmpty(this.Location)) strings.Add(new IndexString("loc:", this.Location));
            return strings;
        }

        /// <summary>
        /// Parse/Split the roots and store them with prefixes and without prefixes
        /// </summary>
        /// <returns>A list of strings ready to be written into indexes</returns>
        public IEnumerable<string> GetSplittedIndexStrings()
        {
            IEnumerable<IndexString> indexStrings = GetIndexStrings();
            List<string> results = new List<string>();
            foreach (IndexString istring in indexStrings)
            {
                // Split the indexStrings except for time
                if (istring.Prefix != "time:")
                {
                    results.AddRange(istring.ToSplittedStrings());
                }
                else
                {
                    results.Add(istring.Root.ToLowerInvariant());
                    results.Add(istring.ToString().ToLowerInvariant());
                }
            }
            return results;
        }
    }
}
