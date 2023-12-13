using Akiled.Database.Interfaces;
using AkiledEmulator.Core;
using MySql.Data.MySqlClient;
using System;
using System.Data;

namespace Akiled.Database.Adapter
{
    public class QueryAdapter : IRegularQueryAdapter
    {
        public IDatabaseClient Client { get; set; }
        public MySqlCommand Command { get; set; }

        private readonly bool _dbEnabled = true;

        public QueryAdapter(IDatabaseClient client) => this.Client = client;

        public void AddParameter(string name, string query) => this.Command.Parameters.AddWithValue(name, query);

        public void AddParameter(string name, int query) => this.Command.Parameters.AddWithValue(name, query.ToString());
        public void AddParameter(string name, object query) => this.Command.Parameters.AddWithValue(name, query);

        public bool FindsResult()
        {
            var hasRows = false;
            try
            {
                using var reader = this.Command.ExecuteReader();
                hasRows = reader.HasRows;
            }
            catch (Exception exception)
            {
                ExceptionLogger.LogQueryError(exception, this.Command.CommandText);
            }

            return hasRows;
        }

        public int GetInteger()
        {
            var result = 0;
            try
            {
                var obj2 = this.Command.ExecuteScalar();
                if (obj2 != null)
                {
                    _ = int.TryParse(obj2.ToString(), out result);
                }
            }
            catch (Exception exception)
            {
                ExceptionLogger.LogQueryError(exception, this.Command.CommandText);
            }

            return result;
        }

        public DataRow GetRow()
        {
            DataRow row = null;
            try
            {
                var dataSet = new DataSet();
                using (var adapter = new MySqlDataAdapter(this.Command))
                {
                    _ = adapter.Fill(dataSet);
                }
                if ((dataSet.Tables.Count > 0) && (dataSet.Tables[0].Rows.Count == 1))
                {
                    row = dataSet.Tables[0].Rows[0];
                }
            }
            catch (Exception exception)
            {
                ExceptionLogger.LogQueryError(exception, this.Command.CommandText);
            }

            return row;
        }

        public string GetString()
        {
            var str = string.Empty;
            try
            {
                var obj2 = this.Command.ExecuteScalar();
                if (obj2 != null)
                {
                    str = obj2.ToString();
                }
            }
            catch (Exception exception)
            {
                ExceptionLogger.LogQueryError(exception, this.Command.CommandText);
            }

            return str;
        }

        public DataTable GetTable()
        {
            var dataTable = new DataTable();
            if (!this._dbEnabled)
            {
                return dataTable;
            }

            try
            {
                using var adapter = new MySqlDataAdapter(this.Command);
                _ = adapter.Fill(dataTable);
            }
            catch (Exception exception)
            {
                ExceptionLogger.LogQueryError(exception, this.Command.CommandText);
            }

            return dataTable;
        }

        public void RunQuery(string query)
        {
            if (!this._dbEnabled)
            {
                return;
            }

            this.SetQuery(query);
            this.RunQuery();
        }

        public void SetQuery(string query)
        {
            this.Command.Parameters.Clear();
            this.Command.CommandText = query;
        }

        public long InsertQuery()
        {
            if (!this._dbEnabled)
            {
                return 0;
            }

            var lastInsertedId = 0L;
            try
            {
                _ = this.Command.ExecuteScalar();
                lastInsertedId = this.Command.LastInsertedId;
            }
            catch (Exception exception)
            {
                ExceptionLogger.LogQueryError(exception, this.Command.CommandText);
            }
            return lastInsertedId;
        }

        public void RunQuery()
        {
            if (!this._dbEnabled)
            {
                return;
            }

            try
            {
                _ = this.Command.ExecuteNonQuery();
            }
            catch (Exception exception)
            {
                ExceptionLogger.LogQueryError(exception, this.Command.CommandText);
            }
        }


    }
}