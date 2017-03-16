using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Emsal.Utility.CustomObjects
{
  public static  class DatetimeExtension
    {
      public static Int64 getInt64Date(this DateTime dtime)
      {
          string guid = dtime.ToString("yyyy") + dtime.ToString("MM") + dtime.ToString("dd") + dtime.ToString("HH") + dtime.ToString("mm") + dtime.ToString("ss") + dtime.ToString("fff");
          System.Threading.Thread.Sleep(1);

          return Int64.Parse(guid);
      }
      public static Int64 getInt64ShortDate(this DateTime dtime)
      {
            string guid = "0";

            if (dtime != DateTime.MinValue) {
                guid = dtime.ToString("yyyy") + dtime.ToString("MM") + dtime.ToString("dd");// +dtime.ToString("HH") + dtime.ToString("mm") + dtime.ToString("ss") + dtime.ToString("fff");
                System.Threading.Thread.Sleep(1);
            }
           
          return Int64.Parse(guid);
      }
        public static DateTime longDate(this Int64 i)
        {
            Int64 n = Int64.Parse(i.ToString("yyyyMMddHHmmss"));
            return DateTime.Parse(n.ToString());
        }
        public static DateTime shortDate(this Int64 i)
        {
            Int64 n = Int64.Parse(i.ToString("yyyyMMdd"));
            return DateTime.Parse(n.ToString());
        }


        public static DateTime toLongDate(this Int64? i)
        {
          
            Int64 newInt = 0;
            try
            {
                if (i != null)
                {
                    newInt = (long)i;
                    //  newInt= Int64.Parse(newInt.ToString("yyyyMMddHHmmssfff"));
                    string a = newInt.ToString().Substring(0, 16);
                    var b =  DateTime.ParseExact(a, "yyyyMMddhhmmssff",new CultureInfo("en-US"));
                    return b;
                }
            }
            catch (Exception ex)
            {

               
            }
            return new DateTime();



        }
        public static DateTime toShortDate(this Int64? i)
        {
            Int64 newInt = 0;
            try
            {
                if (i != null)
                {
                    newInt = (long)i;
                    //  newInt= Int64.Parse(newInt.ToString("yyyyMMddHHmmssfff"));
                    string a = newInt.ToString().Substring(0, 8);
                    var b = DateTime.ParseExact(a, "yyyyMMdd", new CultureInfo("en-US"));
                    return b;
                }
            }
            catch (Exception ex)
            {


            }
            return new DateTime();
        }
    }
}
