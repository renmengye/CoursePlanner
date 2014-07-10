using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Panta.DataModels.Extensions.UT
{
    [DataContract]
    [Serializable]
    public struct UTCourseEvaluation
    {
        [DataMember]
        public double Presentation { get; set; }

        [DataMember]
        public double Explanation { get; set; }

        [DataMember]
        public double Communication { get; set; }

        [DataMember]
        public double Teaching { get; set; }

        [DataMember]
        public double Workload { get; set; }

        [DataMember]
        public double Difficulty { get; set; }

        [DataMember]
        public double LearningExperience { get; set; }

        [DataMember]
        public string Comment { get; set; }
    }
}
