﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LogicLayer.Helpers
{
    public static class DateTimeExt
    {
        /// <summary>
        /// Get time in milliseconds from 1970.01.01
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static long From1970(this DateTime date)
        {
            return (long)(date.Subtract(new DateTime(1970, 1, 1))).TotalMilliseconds;
        }
    }
}
