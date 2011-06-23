using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;
using System.Data.SqlClient;
using System.Data;

namespace CDRUpdater
{
    class SQLConnection
    {
        private MySqlConnection connection;
        private MySqlCommand command;

        public SQLConnection(string connection_string)
        {
            connection = new MySqlConnection(connection_string);
            connection.Open();

            command = connection.CreateCommand();
        }

        public void Execute(string sql)
        {
            command.CommandText = sql;
            command.ExecuteNonQuery();
        }

        public object ExecuteScalar(string sql)
        {
            command.CommandText = sql;
            return command.ExecuteScalar();
        }

        public MySqlDataReader ExecuteReader(string sql)
        {
            command.CommandText = sql;
            return command.ExecuteReader();
        }

        public DataTable ExecuteDataSet(string sql)
        {
            DataSet dataSet = new DataSet();
            command.CommandText = sql;

            MySqlDataAdapter adapter = new MySqlDataAdapter(command);
            adapter.Fill(dataSet);

            return dataSet.Tables[0];
        }
    }
}
