using SharedPacketLib;
using System;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace ConnectionManager
{
    public class GameSocketManager
    {
        private Socket connectionListener;
        private bool acceptConnections;
        private int maxIpConnectionCount;
        private int acceptedConnections;
        private IDataParser parser;
        private ConcurrentDictionary<string, int> _ipConnectionsCount;

        public event ConnectionEvent connectionEvent;

        public void init(int portID, int connectionsPerIP, IDataParser parser, bool DisabledProtect = false)
        {
            this._ipConnectionsCount = new ConcurrentDictionary<string, int>();

            this.parser = parser;
            this.maxIpConnectionCount = connectionsPerIP;
            this.acceptedConnections = 0;

            this.connectionListener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                this.connectionListener.Bind((EndPoint)new IPEndPoint(IPAddress.Any, portID));
                this.connectionListener.Listen(100);
                this.connectionListener.BeginAccept(new AsyncCallback(this.newConnectionRequest), (object)this.connectionListener);
                this.connectionListener.SendBufferSize = GameSocketManagerStatics.BUFFER_SIZE;
                this.connectionListener.ReceiveBufferSize = GameSocketManagerStatics.BUFFER_SIZE;
            }
            catch (Exception ex)
            {
                this.Destroy();
                Console.WriteLine(ex);
                return;
            }

            this.acceptConnections = true;
        }

        public void Destroy()
        {
            this.acceptConnections = false;
            try
            {
                this.connectionListener.Close();
            }
            catch
            {
            }
        }

        private void newConnectionRequest(IAsyncResult iAr)
        {
            if (!this.acceptConnections)
            {
                Console.WriteLine("Connection denied, server is not currently accepting connections!");
                return;
            }
            try
            {
                Socket dataStream = this.connectionListener.EndAccept(iAr);

                string Ip = dataStream.RemoteEndPoint.ToString().Split(new char[1] { ':' })[0];

                int ConnectionCount = getAmountOfConnectionFromIp(Ip);
                if (ConnectionCount < maxIpConnectionCount)
                {
                    Interlocked.Increment(ref this.acceptedConnections);

                    ConnectionInformation connection = new ConnectionInformation(dataStream, this.acceptedConnections, this.parser.Clone() as IDataParser, Ip);

                    connection.connectionClose += new ConnectionInformation.ConnectionChange(this.c_connectionChanged);

                    reportUserLogin(Ip);

                    if (this.connectionEvent != null)
                        this.connectionEvent(connection);
                }
            }
            catch
            {
            }
            finally
            {
                this.connectionListener.BeginAccept(new AsyncCallback(this.newConnectionRequest), (object)this.connectionListener);
            }
        }

        private void c_connectionChanged(ConnectionInformation information)
        {
            this.reportDisconnect(information);
        }

        public void reportDisconnect(ConnectionInformation gameConnection)
        {
            gameConnection.connectionClose -= new ConnectionInformation.ConnectionChange(this.c_connectionChanged);
            reportUserLogout(gameConnection.getIp());
        }

        private void reportUserLogin(string ip)
        {
            alterIpConnectionCount(ip, (getAmountOfConnectionFromIp(ip) + 1));
        }

        private void reportUserLogout(string ip)
        {
            alterIpConnectionCount(ip, (getAmountOfConnectionFromIp(ip) - 1));
        }

        private void alterIpConnectionCount(string ip, int amount)
        {
            if (ip == "127.0.0.1")
                return;

            if (_ipConnectionsCount.ContainsKey(ip))
            {
                int am;
                _ipConnectionsCount.TryRemove(ip, out am);
            }
            _ipConnectionsCount.TryAdd(ip, amount);
        }

        private int getAmountOfConnectionFromIp(string ip)
        {
            try
            {
                if (ip == "127.0.0.1")
                    return 0;

                if (_ipConnectionsCount.TryGetValue(ip, out int Count))
                {
                    return Count;
                }
                else
                {
                    return 0;
                }
            }
            catch
            {
                return 0;
            }
        }

        public delegate void ConnectionEvent(ConnectionInformation connection);
    }
}
