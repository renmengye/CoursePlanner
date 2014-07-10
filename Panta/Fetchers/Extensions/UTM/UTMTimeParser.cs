using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Panta.Fetchers.Extensions.UTM
{
    public static class UTMTimeParser
    {
        public static DayOfWeek ParseDay(string day)
        {
            switch (day)
            {
                case ("MO"):
                    {
                        return DayOfWeek.Monday;
                    }
                case ("TU"):
                    {
                        return DayOfWeek.Tuesday;
                    }
                case ("WE"):
                    {
                        return DayOfWeek.Wednesday;
                    }
                case ("TH"):
                    {
                        return DayOfWeek.Thursday;
                    }
                case ("FR"):
                    {
                        return DayOfWeek.Friday;
                    }
            }
            return DayOfWeek.Monday;
        }
    }
}
