﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;

namespace Emsal.Utility.CustomObjects
{
    public static class Custom
    {
        public static string ConverPriceToStringDelZero(decimal pr)
        {
            string str = "";

            //str = pr.ToString("G29");

            //Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator = ".";

            str = string.Format(Thread.CurrentThread.CurrentCulture = new CultureInfo("de-DE"), "{0:#,##0.000}", pr).TrimEnd('0', '.').TrimEnd(',');

            return str;
        }

    }
}
