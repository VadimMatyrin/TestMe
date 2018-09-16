﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TestMe.Models
{
    public class LoadConfig
    {
        public int TakeAmount { get; set; }
        public int TopRatedHomePageAmount { get; set; }
        public int MinTopRatedRate { get; set; }
        public int MinReportAmount { get; set; }
    }
}
