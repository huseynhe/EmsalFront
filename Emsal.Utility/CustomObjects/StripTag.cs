using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Emsal.Utility.CustomObjects
{
    public static class StripTag
    {
       public static string strSqlBlocker(string StrValue)
        {
            string[] BadCharacters = { "*", "#", ">", "<", "=", "&", "$", "%", "(", ")", "@", "!", ",", "'", "^", "||", "&", ":", "/", "from", "select", "delete", "update", "all" };

            int i;
            for (i = 0;i< BadCharacters.Length; i++) {
                StrValue = StrValue.Replace(BadCharacters[i], "");
            }

            return StrValue;
        }
    }
}
