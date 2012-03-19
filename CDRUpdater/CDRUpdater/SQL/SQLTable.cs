using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;
using System.Diagnostics;

namespace CDRUpdater
{
    class SQLTable<T>
    {
        struct TableRow
        {
            public T data;
            public uint user_data;
        }

        private Dictionary<string, TableRow> RowIndexMap;
        private string[] keys;

        public SQLTable(string[] keys)
        {
            this.keys = keys;

            RowIndexMap = new Dictionary<string,TableRow>();
        }

        // a true multi-key map
        public static string MakeKey(object[] keys)
        {
            return string.Join("|", keys.Select(k => k.ToString()));
        }

        public void Attach(T row, object[] keys, uint user_data)
        {
            TableRow ts = new TableRow()
            {
                data = row,
                user_data = user_data
            };

            RowIndexMap[MakeKey(keys)] = ts;
        }

        public delegate void ProcessFoundCallback(T row, MySqlDataReader reader, uint userid);
        public delegate void ProcessMissingCallback(MySqlDataReader reader);
        public void Process(MySqlDataReader reader, ProcessFoundCallback callback, ProcessMissingCallback callbackMissing)
        {
            while (reader.Read())
            {
                string key = MakeKey(keys.Select(k => reader[k]).ToArray());

                TableRow ts;
                if (RowIndexMap.TryGetValue(key, out ts))
                {
                    // we found the row by the key we were looking for, do update logic
                    callback(ts.data, reader, ts.user_data);
                    RowIndexMap.Remove(key);
                }
                else
                {
                    // this row is in the DB but we don't have a key for it, do closeout logic
                    callbackMissing(reader);
                }
            }

            foreach (TableRow ts in RowIndexMap.Values)
            {
                // we have rows that we didn't find in the DB, do insert logic
                callback(ts.data, null, ts.user_data);
            }

            RowIndexMap.Clear();
        }
    }
}
