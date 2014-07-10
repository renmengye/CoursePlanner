using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Panta.Fetchers.Extensions.UT
{
    /// <summary>
    /// Comparer for incomplete course abbreviation code
    /// </summary>
    public class UTCourseAbbrComparer : EqualityComparer<string>
    {
        public override bool Equals(string x, string y)
        {
            return x.StartsWith(y) || y.StartsWith(x) || x == y;
        }

        public override int GetHashCode(string obj)
        {
            if (obj.Length >= 8)
            {
                return obj.Substring(0, 8).GetHashCode();
            }
            else
            {
                return obj.GetHashCode();
            }
        }
    }
}
