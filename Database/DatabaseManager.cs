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

            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            try
            {

                MySqlConnection con = new MySqlConnection(this._connectionStr);
                con.Open();
                MySqlCommand CMD = con.CreateCommand();
                CMD.CommandText = "SELECT 1+1";
                CMD.ExecuteNonQuery();

                CMD.Dispose();
                con.Close();
            }
            catch (MySqlException)
            {
                return false;
            }

            return true;
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