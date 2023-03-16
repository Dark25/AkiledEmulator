using Akiled.Database.Adapter;
using Akiled.Database.Interfaces;
using MySql.Data.MySqlClient;
using System;
using System.Data;

namespace Akiled.Database
{
    public class DatabaseConnection : IDatabaseClient, IDisposable
    {
        private readonly IQueryAdapter _adapter;
        private readonly MySqlConnection _con;

        public DatabaseConnection(string ConnectionStr)
        {
            this._con = new MySqlConnection(ConnectionStr);
            this._adapter = new NormalQueryReactor(this);
        }

        public void Dispose()
        {
            if (this._con.State == ConnectionState.Open)
            {
                this._con.Close();
            }

            this._con.Dispose();
            GC.SuppressFinalize(this);
        }

        public void connect()
        {
            this.Open();
        }

        public void disconnect()
        {
            this.Close();
        }

        public IQueryAdapter getQueryreactor()
        {
            return this._adapter;
        }

        public void prepare()
        {
            // nothing here
        }

        public void reportDone()
        {
            Dispose();
        }

        public MySqlCommand createNewCommand()
        {
            return _con.CreateCommand();
        }

        public void Open()
        {
            if (_con.State == ConnectionState.Closed)
            {
                try
                {
                    _con.Open();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
            }
        }

        public void Close()
        {
            if (_con.State == ConnectionState.Open)
            {
                _con.Close();
            }
        }
    }
}