using Akiled.Core;
using Akiled.Net;
using ConnectionManager;
using System;

namespace Akiled.Communication.WebSocket
{
    public class WebSocketManager
    {
        public GameSocketManager manager;

        public WebSocketManager(int port, int maxConnections)
        {
            this.manager = new GameSocketManager();
            this.manager.init(port, maxConnections, new InitialPacketParser());

            this.manager.connectionEvent += new GameSocketManager.ConnectionEvent(this.ConnectionEvent);
        }

        private void ConnectionEvent(ConnectionInformation connection)
        {
            connection.connectionClose += new ConnectionInformation.ConnectionChange(this.ConnectionChanged);

            AkiledEnvironment.GetGame().GetClientWebManager().CreateAndStartClient(connection.getConnectionID(), connection);
        }

        private void ConnectionChanged(ConnectionInformation information)
        {
            this.CloseConnection(information);
            information.connectionClose -= new ConnectionInformation.ConnectionChange(this.ConnectionChanged);
        }

        private void CloseConnection(ConnectionInformation Connection)
        {
            try
            {
                AkiledEnvironment.GetGame().GetClientWebManager().DisposeConnection(Connection.getConnectionID());
                Connection.Dispose();
            }
            catch (Exception ex)
            {
                Logging.LogException((ex).ToString());
            }
        }

        public void Destroy()
        {
            this.manager.Destroy();
        }
    }
}
