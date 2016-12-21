using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Emsal.Utility.CustomObjects
{
    public static class Custom
    {
        public static string ConverPriceToStringDelZero(decimal pr)
        {
            string str = "";
            str = pr.ToString("G29");
            return str;
        }

    }
}
