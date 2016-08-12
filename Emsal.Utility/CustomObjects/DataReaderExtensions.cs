using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Emsal.Utility.CustomObjects
{
    public static class DataReaderExtensions
    {
        public static string GetStringOrEmpty(this IDataReader reader, int ordinal)
        {
            return reader.IsDBNull(ordinal) ? String.Empty : reader.GetString(ordinal);
        }

        public static string GetStringOrEmpty(this IDataReader reader, string columnName)
        {
            return reader.GetStringOrEmpty(reader.GetOrdinal(columnName));
        }

        public static Int64 GetInt64OrDefaultValue(this IDataReader reader, int ordinal)
        {
            return reader.IsDBNull(ordinal) ? 0 : reader.GetInt64(ordinal);
        }

        public static Int64 GetInt64OrDefaultValue(this IDataReader reader, string columnName)
        {
            return reader.GetInt64OrDefaultValue(reader.GetOrdinal(columnName));
        }

        public static decimal GetDecimalOrDefaultValue(this IDataReader reader, int ordinal)
        {
            return reader.IsDBNull(ordinal) ? 0 : reader.GetDecimal(ordinal);
        }

        public static decimal GetDecimalOrDefaultValue(this IDataReader reader, string columnName)
        {
            return reader.GetDecimalOrDefaultValue(reader.GetOrdinal(columnName));
        }
    }
}
