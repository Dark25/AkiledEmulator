using Akiled.Database.Interfaces;
using MySql.Data.MySqlClient;
using System;


namespace Akiled.Database
{
    public sealed class DatabaseManager
    {
        private readonly string _connectionStr;

        public DatabaseManager(uint DbPoolMax, uint DbPoolMin, string DbHostname, uint DbPort, string DbUsername, string DbPassword, string DbName)
        {
            MySqlConnectionStringBuilder connectionString = new MySqlConnectionStringBuilder
            {
                ConnectionTimeout = 10,
                Database = DbName,
                DefaultCommandTimeout = 30,
                //Logging = false,
                MaximumPoolSize = DbPoolMax,
                MinimumPoolSize = DbPoolMin,
                Password = DbPassword,
                Pooling = true,
                Port = DbPort,
                Server = DbHostname,
                UserID = DbUsername,
                AllowZeroDateTime = true,
                ConvertZeroDateTime = true,
                SslMode = MySqlSslMode.None
            };
            this._connectionStr = connectionString.ToString();
        }

        public bool IsConnected()
        {
            try
            {
                using (MySqlConnection con = new MySqlConnection(this._connectionStr))
                {
                    con.Open();
                    // Just opening is enough to test connectivity with pooling
                    // Execute scalar is minimal overhead compared to Open
                    return con.State == System.Data.ConnectionState.Open;
                }
            }
            catch (MySqlException)
            {
                return false;
            }
        }

        public IQueryAdapter GetQueryReactor()
        {
            try
            {
                IDatabaseClient DbConnection = new DatabaseConnection(this._connectionStr);

                DbConnection.connect();

                return DbConnection.getQueryreactor();
            }
            catch (Exception e)
            {
                LoggingMySql.LogException(e.ToString());
                return null;
            }
        }
    }
}