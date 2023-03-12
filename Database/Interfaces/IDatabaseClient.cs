using System;
using MySqlConnector;


namespace Akiled.Database.Interfaces
{
    public interface IDatabaseClient : IDisposable
    {
        void connect();
        void disconnect();
        IQueryAdapter getQueryreactor();
        MySqlCommand createNewCommand();
        void reportDone();
    }
}