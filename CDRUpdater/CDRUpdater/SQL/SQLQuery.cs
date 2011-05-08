using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SteamKit2;
using Jayrock.Json.Conversion;
using System.Data;

namespace CDRUpdater
{
    static class SQLQuery
    {
        private static CacheContext SQLContext = new CacheContext();

        /*public static string BuildDataInsertFromType(object x)
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

            return String.Join("\t", sqlDict.Values);
        }*/

        public static void BuildDataInsertFromTypeWithChanges(object x, DataRow prev_data, int cdr_id, int prev_cdr_id, out string sql_data, out string sql_data_capture)
        {
            Type otype = x.GetType();
            Dictionary<string, string> sqlDict = new Dictionary<string, string>();
            Dictionary<string, string> sqlCaptureDict = new Dictionary<string, string>();
            int capture_count = -1;

            foreach (var field in otype.GetCachedPropertyInfo(SQLContext))
            {
                SqlColumnAttribute cattrib = field.GetAttribute<SqlColumnAttribute>(SQLContext);

                if (cattrib == null)
                    continue;

                bool enclose = false;
                string type_value = GetStringValue(field.PropertyType, field.GetValue(x, null), true, out enclose);

                if (prev_data != null)
                {
                    bool inner_enclose;

                    string value_current = GetStringValue(field.PropertyType, prev_data[cattrib.Column], false, out inner_enclose);

                    if (value_current != type_value || capture_count < 0)
                    {
                        sqlCaptureDict.Add(cattrib.Column, EscapeValue(value_current, inner_enclose));
                        capture_count++;
                    }
                    else
                    {
                        sqlCaptureDict.Add(cattrib.Column, @"\N");
                    }
                }

                type_value = EscapeValue(type_value, enclose);

                sqlDict.Add(cattrib.Column, type_value);

            }

            sql_data = String.Join("\t", sqlDict.Values);

            if (prev_data == null || capture_count <= 0)
            {
                sql_data_capture = null;
                return;
            }

            sql_data_capture = String.Format("{0}\t{1}", prev_cdr_id, String.Join("\t", sqlCaptureDict.Values));
        }

        private static string GetStringValue(Type propType, object value, bool follow_types, out bool enclose)
        {
            string data = null;
            enclose = false;

            if (propType.IsEnum)
            {
            }

            Type generic;
            int genericCount;

            if (follow_types && propType.IsTypeListOrDictionary(out generic, out genericCount))
            {
                if (value == null)
                    value = new object[] { };

                data = JsonConvert.ExportToString(value);
                enclose = true;
            }
            else
            {
                if (propType == typeof(string))
                    enclose = true;

                if (propType == typeof(bool))
                    data = ((bool)value) ? "1" : "0";
                else
                    data = Convert.ToString(value);
            }

            return data;
        }

        private static string EscapeValue(string input, bool enclose)
        {
            string clean = input.Replace(@"\", @"\\").Replace("\0", "\\0").Replace("\'", "\\\'").Replace("\"", "\\\"");

            if (enclose)
                return clean; //'"' + clean + '"';
            else
                return clean;
        }
    }
}
