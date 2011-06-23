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
        struct TStruct
        {
            public T data;
            public uint user_id;
        }

        private Dictionary<int, TStruct> RowIndexMap;
        private string[] keys;

        public SQLTable(string[] keys)
        {
            this.keys = keys;

            RowIndexMap = new Dictionary<int,TStruct>();
        }

        public void Attach(T row, int index, uint userid)
        {
            //Debug.Assert(RowIndexMap.ContainsKey(index) == false, "Duplicate key");

            TStruct ts = new TStruct()
            {
                data = row,
                user_id = userid
            };

            RowIndexMap[index] = ts;
        }

        public delegate void ProcessFoundCallback(T row, MySqlDataReader reader, uint userid);
        public delegate void ProcessMissingCallback(MySqlDataReader reader);
        public void Process(MySqlDataReader reader, ProcessFoundCallback callback, ProcessMissingCallback callbackMissing)
        {
            while (reader.Read())
            {
                string tohash = reader[keys[0]].ToString();

                for (int i = 1; i < keys.Length; i++) tohash += reader[keys[i]].ToString();

                int hash = tohash.GetHashCode();

                TStruct ts;
                if (RowIndexMap.TryGetValue(hash, out ts))
                {
                    callback(ts.data, reader, ts.user_id);
                    RowIndexMap.Remove(hash);
                }
                else
                {
                    callbackMissing(reader);
                }
            }

            foreach (TStruct ts in RowIndexMap.Values)
            {
                callback(ts.data, null, ts.user_id);
            }
        }
    }
}
