using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SteamKit2;
using Jayrock.Json.Conversion;

namespace CDRUpdater
{
    static class SQLQuery
    {
        private static CacheContext SQLContext = new CacheContext();

        public static string BuildInsertQueryFromType(string table, object x)
        {
            Type otype = x.GetType();
            Dictionary<string, string> sqlDict = new Dictionary<string, string>();

            foreach (var field in otype.GetCachedPropertyInfo(SQLContext))
            {
                SqlColumnAttribute cattrib = field.GetAttribute<SqlColumnAttribute>(SQLContext);

                if (cattrib == null)
                    continue;

                sqlDict.Add(cattrib.Column, GetStringValue(field.PropertyType, field.GetValue(x, null)));
            }

            return String.Format("INSERT INTO {2} ({0}) VALUES ({1});", String.Join(", ", sqlDict.Keys), String.Join(", ", sqlDict.Values), table);
        }

        private static string GetStringValue(Type propType, object value)
        {
            string data = null;

            if (propType.IsEnum)
            {
            }

            Type generic;
            int genericCount;

            if (propType.IsTypeListOrDictionary(out generic, out genericCount))
            {
                if (value == null)
                    value = new object[] { };

                data = JsonConvert.ExportToString(value);
            }
            else
            {
                data = Convert.ToString(value);
            }

            return EscapeValue(data);
        }

        private static string EscapeValue(string input)
        {
            return '"' + input.Replace("\0", "\\0").Replace("\\", "\\\\").Replace("\'", "\\\'").Replace("\"", "\\\"") + '"';
        }
    }
}
