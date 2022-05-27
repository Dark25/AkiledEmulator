using Akiled;
using Akiled.Core;
using Akiled.Net;
using System;

namespace ConnectionManager
{
    public class ConnectionHandeling
    {
        public GameSocketManager manager;

        public ConnectionHandeling(int port, int maxConnections, int connectionsPerIP)
        {
            this.manager = new GameSocketManager();
            this.manager.init(port, maxConnections, new InitialPacketParser());

            this.manager.connectionEvent += new GameSocketManager.ConnectionEvent(this.ConnectionEvent);
        }

        private void ConnectionEvent(ConnectionInformation connection)
        {
            connection.connectionClose += new ConnectionInformation.ConnectionChange(this.ConnectionChanged);

            AkiledEnvironment.GetGame().GetClientManager().CreateAndStartClient(connection.getConnectionID(), connection);
        }

        private void ConnectionChanged(ConnectionInformation information)
        {
            this.CloseConnection(information);

            information.connectionClose -= new ConnectionInformation.ConnectionChange(this.ConnectionChanged);
        }

        public void CloseConnection(ConnectionInformation Connection)
        {
            try
            {
                AkiledEnvironment.GetGame().GetClientManager().DisposeConnection(Connection.getConnectionID());

                Connection.Dispose();
            }
            catch (Exception ex)
            {
                Logging.LogException((ex).ToString());
            }
        }


        public void destroy()
        {
            this.manager.Destroy();
        }
    }
}
