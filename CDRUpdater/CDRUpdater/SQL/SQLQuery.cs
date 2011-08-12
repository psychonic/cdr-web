using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SteamKit2;
using Jayrock.Json.Conversion;
using System.Data;
using System.IO;
using MySql.Data.MySqlClient;

namespace CDRUpdater
{
    static class SQLQuery
    {
        private static CacheContext SQLContext = new CacheContext();


        public static void BuildDataInsertFromTypeWithChanges(object x, MySqlDataReader prev_data, int cdr_id, /*int prev_cdr_id,*/ out string sql_data, out string sql_data_capture)
        {
            Type otype = x.GetType();
            List<string> sqlDict = new List<string>();
            List<string> sqlCaptureDict = new List<string>();
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
                        sqlCaptureDict.Add(EscapeValue(value_current, inner_enclose));
                        capture_count++;
                    }
                    else
                    {
                        sqlCaptureDict.Add(@"\N");
                    }
                }

                type_value = EscapeValue(type_value, enclose);

                sqlDict.Add(type_value);

            }

            sql_data = String.Join("\t", sqlDict);

            if (prev_data == null || capture_count <= 0)
            {
                sql_data_capture = null;
                return;
            }

            sql_data_capture = String.Format("{0}\t0\t{1}", /*prev_cdr_id,*/ cdr_id, String.Join("\t", sqlCaptureDict));
        }

        public static void BuildSubDataInsertFromType(string table, object x, MySqlDataReader prev_data, uint appID, int cdr_id, int prev_cdr_id, StreamWriter writer)
        {
            Type otype = x.GetType();
            List<string> sqlDict = new List<string>();
            bool newData = (prev_data == null);

            foreach (var field in otype.GetCachedPropertyInfo(SQLContext))
            {
                SqlColumnAttribute cattrib = field.GetAttribute<SqlColumnAttribute>(SQLContext);

                if (cattrib == null)
                    continue;

                bool enclose = false;
                string type_value = GetStringValue(field.PropertyType, field.GetValue(x, null), true, out enclose);

                bool inner_enclose = false;

                if (!newData && CompareTranslateBool(GetStringValue(field.PropertyType, prev_data[cattrib.Column], false, out inner_enclose), type_value) == false)
                {
                    newData = true;
                }

                type_value = EscapeValue(type_value, enclose);

                sqlDict.Add(type_value);
            }

            if (!newData)
                return;

            if (prev_data != null)
            {
                // close out current data
                CloseoutRow(otype, prev_data, prev_cdr_id, writer);
            }

            writer.Write("{0}\t{1}\t\\N\t{2}\r\n", appID, cdr_id, String.Join("\t", sqlDict));
        }

        public static void CloseoutRow(Type otype, MySqlDataReader prev_data, int cdr_id_last, StreamWriter writer)
        {
            List<string> sqlDict = new List<string>();

            foreach (var field in otype.GetCachedPropertyInfo(SQLContext))
            {
                SqlColumnAttribute cattrib = field.GetAttribute<SqlColumnAttribute>(SQLContext);

                if (cattrib == null)
                    continue;

                bool enclose = false;
                string type_value = GetStringValue(field.PropertyType, prev_data[cattrib.Column], false, out enclose);

                sqlDict.Add(EscapeValue(type_value, enclose));
            }

            writer.Write("{0}\t{1}\t{2}\t{3}\r\n", prev_data["app_id"], prev_data["cdr_id"], cdr_id_last, String.Join("\t", sqlDict));
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

        public static string EscapeValue(string input, bool enclose)
        {
            string clean = input.Replace(@"\", @"\\").Replace("\0", "\\0").Replace("\'", "\\\'").Replace("\"", "\\\"");

            if (enclose)
                return clean; //'"' + clean + '"';
            else
                return clean;
        }

        // pretty
        private static bool CompareTranslateBool(string x, string y)
        {
            if (x.Equals(y))
                return true;

            if ((x == "False" || x == "True") && (x.Replace("True", "1").Replace("False", "0").Equals(y)))
                return true;

            return false;
        }
    }
}
