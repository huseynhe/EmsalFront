using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;

namespace Emsal.Utility.CustomObjects
{
    public static class Custom
    {
        public static string ConverPriceToStringDelZero(decimal? pr)
        {
            string str = "";
 
            //str = pr.ToString("G29");
            //return Decimal.Parse(str);

            //Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator = ".";

            //str = string.Format(Thread.CurrentThread.CurrentCulture = new CultureInfo("de-DE"), "{0:#,##0.000}", pr).TrimEnd('0', '.').TrimEnd(',');

            str = string.Format(Thread.CurrentThread.CurrentCulture = new CultureInfo("az-Latn-AZ"), "{0:#,##0.00}", pr).TrimEnd('0', '.').TrimEnd(',');

            //str = string.Format(Thread.CurrentThread.CurrentCulture = new CultureInfo("az-Latn-AZ"), "{0:#,##0.000000000}", pr).TrimEnd('0', ',').TrimEnd('.');

            //str = str.Replace(",", ".");

            return str;
        }

        public static decimal ConverPriceDelZero(decimal pr)
        {
            string str = "";

            str = pr.ToString("G29");
            return Math.Round(Decimal.Parse(str) , 2);

            //Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator = ".";

            //str = string.Format(Thread.CurrentThread.CurrentCulture = new CultureInfo("de-DE"), "{0:#,##0.000}", pr).TrimEnd('0', '.').TrimEnd(',');

            //str = string.Format(Thread.CurrentThread.CurrentCulture = new CultureInfo("az-Latn-AZ"), "{0:#,##0.000000}", pr).TrimEnd('0', '.').TrimEnd(',');

            //str = str.Replace(",", ".");

            //return str;
        }

    }
}
