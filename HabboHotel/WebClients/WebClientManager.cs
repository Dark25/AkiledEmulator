using Akiled.Communication.Packets.Outgoing;
using Akiled.Core;
using ConnectionManager;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Akiled.HabboHotel.WebClients
{
    public class WebClientManager
    {
        public ConcurrentDictionary<int, WebClient> _clients;
        public ConcurrentDictionary<int, int> _userIDRegister;

        public int Count
        {
            get
            {
                return this._clients.Count;
            }
        }

        public WebClientManager()
        {
            this._clients = new ConcurrentDictionary<int, WebClient>();
            this._userIDRegister = new ConcurrentDictionary<int, int>();
        }

        public WebClient GetClientByUserID(int userID)
        {
            if (this._userIDRegister.ContainsKey(userID))
            {
                if (!TryGetClient(this._userIDRegister[userID], out WebClient Client)) return null;

                return Client;
            }
            else
                return null;
        }

        public bool TryGetClient(int ClientId, out WebClient Client)
        {
            return this._clients.TryGetValue(ClientId, out Client);
        }

        public void SendMessage(IServerPacket Packet, Language Langue = Language.FRANCAIS)
        {
            foreach (WebClient Client in this._clients.Values.ToList())
            {
                if (Client == null || Client.Langue != Langue)
                    continue;

                Client.SendPacket(Packet);
            }
        }

        public void CreateAndStartClient(int clientID, ConnectionInformation connection)
        {
            WebClient Client = new WebClient(clientID, connection);
            if (this._clients.TryAdd(Client.ConnectionID, Client))
                Client.StartConnection();
            else
                connection.Dispose();
        }

        public void DisposeConnection(int clientID)
        {
            if (!TryGetClient(clientID, out WebClient Client))
                return;

            if (Client != null)
                Client.Dispose();

            this._clients.TryRemove(clientID, out Client);
        }

        public void LogClonesOut(int UserID)
        {
            WebClient clientByUserId = this.GetClientByUserID(UserID);
            if (clientByUserId == null)
                return;

            clientByUserId.Dispose();
        }

        public void RegisterClient(WebClient client, int userID)
        {
            if (_userIDRegister.ContainsKey(userID))
                _userIDRegister[userID] = client.ConnectionID;
            else
                _userIDRegister.TryAdd(userID, client.ConnectionID);

            /*using (IQueryAdapter queryreactor = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                queryreactor.RunQuery("UPDATE users SET web_online = '1' WHERE id = " + userID);
            }*/
        }

        public void UnregisterClient(int userid, string username)
        {
            this._userIDRegister.TryRemove(userid, out int Client);
        }

        public void CloseAll()
        {
            try
            {
                foreach (WebClient client in this.GetClients.ToList())
                {

                    if (client == null || client.GetConnection() == null)
                        continue;
                    try
                    {
                        client.GetConnection().Dispose();
                    }
                    catch
                    {
                    }
                }
            }
            catch (Exception ex)
            {
                Logging.LogCriticalException((ex).ToString());
            }
            this._clients.Clear();
        }

        public ICollection<WebClient> GetClients
        {
            get
            {
                return this._clients.Values;
            }
        }
    }
}
