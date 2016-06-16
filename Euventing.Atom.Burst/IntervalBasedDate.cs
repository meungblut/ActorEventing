﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Euventing.Atom.Burst
{
    public class IntervalBasedDate
    {
        private TimeSpan timeSpan;
        private DateTime currentData;

        public IntervalBasedDate(TimeSpan timeSpan)
        {
            this.timeSpan = timeSpan;
        }

        public DateTime Current => GetCurrentDate();

        private DateTime GetCurrentDate()
        {
            var date = Date.CurrentDate();
            var seconds = date.Second - date.Second % timeSpan.Seconds;
            currentData = new DateTime(date.Year, date.Month, date.Day, date.Hour, date.Minute, seconds);
            return currentData;
        }
    }
}
