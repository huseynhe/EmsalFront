using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Emsal.Utility.CustomObjects
{
    public static class StringExtension
    {
        // This is the extension method.
        // The first parameter takes the "this" modifier
        // and specifies the type for which the method is defined.
        public static int WordCount(this String str)
        {
            return str.Split(new char[] { ' ', '.', '?' }, StringSplitOptions.RemoveEmptyEntries).Length;
        }


        public static Byte[] StringToByteArray(String str)
        {
            string[] bp = str.Split(',').ToArray();
            byte[] bt = new byte[bp.Length];

            for (int i = 0; i < bp.Length; i++)
            {
                bt[i] = Convert.ToByte(bp[i]);
            }

            return bt;
        }

    }
}
