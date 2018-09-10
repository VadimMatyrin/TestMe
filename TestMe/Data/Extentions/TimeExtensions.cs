using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TestMe.Data.Extentions
{
    public static class TimeExtensions
    {
        public static TimeSpan StripMilliseconds(this TimeSpan time)
        {
            return new TimeSpan(time.Days, time.Hours, time.Minutes, time.Seconds);
        }
    }
}
