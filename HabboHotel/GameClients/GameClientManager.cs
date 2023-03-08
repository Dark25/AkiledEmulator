using Akiled.Communication.Packets.Outgoing;
using Akiled.Communication.Packets.Outgoing.Structure;
using Akiled.Core;
using Akiled.Database.Interfaces;
using Akiled.HabboHotel.Rooms;
using Akiled.HabboHotel.Users.Messenger;
using Akin.HabboHotel.Misc;
using ConnectionManager;
using JNogueira.Discord.Webhook.Client;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Akiled.HabboHotel.GameClients
{
    public class GameClientManager
    {

        public ConcurrentDictionary<int, GameClient> _clients;
        public ConcurrentDictionary<string, int> _usernameRegister;
        public ConcurrentDictionary<int, int> _userIDRegister;
        private readonly Queue diamondsQueuee;
        private readonly Queue moedasQueuee;
        public int OnlineUsersFr;
        public int OnlineUsersEn;
        public int OnlineUsersBr;
        public int OnlineUsersEs;

        private readonly List<int> _userStaff;


        public int Count => _userIDRegister.Count;

        public GameClientManager()
        {
            this._clients = new ConcurrentDictionary<int, GameClient>();
            this._usernameRegister = new ConcurrentDictionary<string, int>();
            this._userIDRegister = new ConcurrentDictionary<int, int>();
            this._userStaff = new List<int>();
            this.diamondsQueuee = new Queue();
            this.moedasQueuee = new Queue();
        }

        public List<GameClient> GetStaffUsers()
        {
            List<GameClient> Users = new List<GameClient>();

            foreach (int UserId in this._userStaff)
            {
                GameClient Client = this.GetClientByUserID(UserId);
                if (Client == null || Client.GetHabbo() == null)
                    continue;

                Users.Add(Client);
            }

            return Users;
        }

        public GameClient GetClientByUserID(int userID)
        {
            if (this._userIDRegister.ContainsKey(userID))
            {
                GameClient Client = null;
                if (!TryGetClient(this._userIDRegister[userID], out Client))
                    return null;
                return Client;
            }
            else
                return (GameClient)null;
        }

        public GameClient GetClientByUsername(string username)
        {
            if (!this._usernameRegister.ContainsKey(username.ToLower()))
                return (GameClient)null;
            return !this.TryGetClient(this._usernameRegister[username.ToLower()], out GameClient Client) ? (GameClient)null : Client;
        }

        public int GetUserIdByUsername(string Username)
        {
            GameClient client = GetClientByUsername(Username);

            if (client != null)
                return client.GetHabbo().Id;

            int userid;
            using (IQueryAdapter dbClient = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT id FROM users WHERE username = @username LIMIT 1");
                dbClient.AddParameter("username", Username);
                userid = dbClient.GetInteger();
            }

            return userid;
        }

        public bool UpdateClientUsername(int ClientId, string OldUsername, string NewUsername)
        {
            if (!_usernameRegister.ContainsKey(OldUsername.ToLower()))
                return false;

            _usernameRegister.TryRemove(OldUsername.ToLower(), out ClientId);
            _usernameRegister.TryAdd(NewUsername.ToLower(), ClientId);
            return true;
        }

        public bool TryGetClient(int ClientId, out GameClient Client) => this._clients.TryGetValue(ClientId, out Client);

        public string GetNameById(int Id)
        {
            GameClient clientByUserId = this.GetClientByUserID(Id);

            if (clientByUserId != null) return clientByUserId.GetHabbo().Username;

            string username = "";
            using (IQueryAdapter queryreactor = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                queryreactor.SetQuery("SELECT username FROM users WHERE id = " + Id);
                username = queryreactor.GetString();
            }

            return username;
        }

        public List<GameClient> GetClientsById(Dictionary<int, MessengerBuddy>.KeyCollection users)
        {
            List<GameClient> ClientOnline = new List<GameClient>();
            foreach (int userID in users)
            {
                GameClient client = this.GetClientByUserID(userID);
                if (client != null)
                    ClientOnline.Add(client);
            }

            return ClientOnline;
        }

        public void SendMessage(ServerPacket Packet, string fuse = "")
        {
            foreach (GameClient Client in this._clients.Values.ToList())
            {
                if (Client == null || Client.GetHabbo() == null)
                    continue;

                if (!string.IsNullOrEmpty(fuse))
                {
                    if (!Client.GetHabbo().HasFuse(fuse))
                        continue;
                }

                Client.SendMessage(Packet);
            }
        }


        public void Chatstaff(ServerPacket Message, int Exclude = 0)
        {
            foreach (GameClient client in this.GetClients.ToList())
            {
                if (client == null || client.GetHabbo() == null)
                    continue;

                if (client.GetHabbo().Rank <= 10 || client.GetHabbo().Id == Exclude)
                    continue;

                client.SendPacket(Message);
            }
        }


        public void Chatstaffmods(ServerPacket Message, int Exclude = 0)
        {
            foreach (GameClient client in this.GetClients.ToList())
            {
                if (client == null || client.GetHabbo() == null)
                    continue;

                if (client.GetHabbo().Id == Exclude || !client.GetHabbo().Ismod)
                    continue;

                client.SendPacket(Message);
            }
        }
        public void Chatstaffguias(ServerPacket Message, int Exclude = 0)
        {
            foreach (GameClient client in this.GetClients.ToList())
            {
                if (client == null || client.GetHabbo() == null)
                    continue;

                if (client.GetHabbo().Id == Exclude || !client.GetHabbo().Isguia)
                    continue;

                client.SendPacket(Message);
            }
        }
        public void Chatstaffpubs(ServerPacket Message, int Exclude = 0)
        {
            foreach (GameClient client in this.GetClients.ToList())
            {
                if (client == null || client.GetHabbo() == null)
                    continue;

                if (client.GetHabbo().Id == Exclude || !client.GetHabbo().Ispub)
                    continue;

                client.SendPacket(Message);
            }
        }

        public void Chatstaffgms(ServerPacket Message, int Exclude = 0)
        {
            foreach (GameClient client in this.GetClients.ToList())
            {
                if (client == null || client.GetHabbo() == null)
                    continue;

                if (client.GetHabbo().Id == Exclude || !client.GetHabbo().Isgm)
                    continue;

                client.SendPacket(Message);
            }
        }

        public void Chatstaffinter(ServerPacket Message, int Exclude = 0)
        {
            foreach (GameClient client in this.GetClients.ToList())
            {
                if (client == null || client.GetHabbo() == null)
                    continue;

                if (client.GetHabbo().Id == Exclude || !client.GetHabbo().Isinter)
                    continue;

                client.SendPacket(Message);
            }
        }

        public void StaffAlert(ServerPacket Message, int Exclude = 0)
        {
            foreach (GameClient client in this.GetClients.ToList())
            {
                if (client == null || client.GetHabbo() == null)
                    continue;

                if (client.GetHabbo().Rank < 4 || client.GetHabbo().Id == Exclude)
                    continue;

                client.SendPacket(Message);
            }
        }

        private Stopwatch OnlineTimeStopwatch;
        public void CheckOnlineTime()
        {
            foreach (GameClient client in _clients.Values)
            {
                if (client == null || client.GetHabbo() == null)
                    continue;

                Diamantes.GiveCycleDiamonds(client);
                //Creditos.GiveCycleDiamonds(client);
            }

            OnlineTimeStopwatch.Restart();
        }


        public void StaffAlertSecret(ServerPacket Message, int Exclude = 0)
        {
            foreach (GameClient client in this.GetClients.ToList())
            {
                if (client == null || client.GetHabbo() == null)
                    continue;

                if (client.GetHabbo().Rank < 4 || client.GetHabbo().Id == Exclude)
                    continue;

                client.SendPacket(Message);
            }
        }
        public void StaffWhisper(string Text, int Colour = 0, int Exclude = 0)
        {
            foreach (GameClient Client in this.GetClients.ToList())
            {
                if (Client == null || Client.GetHabbo() == null)
                    continue;

                if (Client.GetHabbo().Rank < 4 || Client.GetHabbo().Id == Exclude)
                    continue;

                RoomUser User = Client.GetHabbo().CurrentRoom.GetRoomUserManager().GetRoomUserByHabboId(Client.GetHabbo().Id);
                if (User == null)
                    return;

                Client.SendPacket(new WhisperMessageComposer(User.VirtualId, Text, 0, (Colour == 0 ? User.LastBubble : Colour)));

            }
        }
        public void SendMessageStaff(IServerPacket Packet)
        {
            foreach (int UserId in this._userStaff)
            {
                GameClient Client = this.GetClientByUserID(UserId);
                if (Client == null || Client.GetHabbo() == null)
                    continue;

                Client.SendPacket(Packet);
            }
        }

        public void SendMessage(IServerPacket Packet)
        {
            foreach (GameClient Client in this._clients.Values.ToList())
            {
                if (Client == null || Client.GetHabbo() == null)
                    continue;

                Client.SendPacket(Packet);
            }
        }

        public void CreateAndStartClient(int clientID, ConnectionInformation connection)
        {
            GameClient Client = new GameClient(clientID, connection);
            if (this._clients.TryAdd(Client.ConnectionID, Client))
                Client.StartConnection();
            else
                connection.Dispose();
        }

        public async void DisposeConnection(int clientID)
        {
            GameClient Client = null;
            if (!TryGetClient(clientID, out Client))
                return;

            if (Client != null)
                await Client.Dispose().ConfigureAwait(true);

            this._clients.TryRemove(clientID, out Client);
        }

        public void LogClonesOut(int UserID)
        {
            GameClient clientByUserId = this.GetClientByUserID(UserID);
            if (clientByUserId == null)
                return;
            clientByUserId.Disconnect();
        }

        public void RegisterClient(GameClient client, int userID, string username)
        {
            if (_usernameRegister.ContainsKey(username.ToLower()))
                _usernameRegister[username.ToLower()] = client.ConnectionID;
            else
                _usernameRegister.TryAdd(username.ToLower(), client.ConnectionID);

            if (_userIDRegister.ContainsKey(userID))
                _userIDRegister[userID] = client.ConnectionID;
            else
                _userIDRegister.TryAdd(userID, client.ConnectionID);
        }

        public void UnregisterClient(int userid, string username)
        {
            int Client = 0;
            this._userIDRegister.TryRemove(userid, out Client);
            this._usernameRegister.TryRemove(username.ToLower(), out Client);
        }

        public void AddUserStaff(int UserId)
        {
            if (!this._userStaff.Contains(UserId))
                this._userStaff.Add(UserId);
        }

        public void RemoveUserStaff(int UserId)
        {
            if (this._userStaff.Contains(UserId))
                this._userStaff.Remove(UserId);
        }

        public void CloseAll()
        {
            StringBuilder stringBuilder = new();

            foreach (GameClient client in this.GetClients.ToList())
            {
                if (client == null)
                    continue;

                if (client.GetHabbo() != null)
                {
                    try
                    {
                        stringBuilder.Append(client.GetHabbo().GetQueryString);
                    }
                    catch
                    {
                    }
                }
            }
            try
            {
                if (stringBuilder.Length > 0)
                {
                    using (IQueryAdapter queryreactor = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
                        queryreactor.RunQuery((stringBuilder).ToString());
                }
            }
            catch (Exception ex)
            {
                Logging.HandleException(ex, "GameClientManager.CloseAll()");
            }
            Console.WriteLine("Done saving users inventory!");
            Console.WriteLine("Closing server connections...");
            try
            {
                foreach (GameClient client in this.GetClients.ToList())
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
            Console.WriteLine("Connections closed!");
        }

        public Task BanUserAsync(GameClient Client, string Moderator, double LengthSeconds, string Reason, bool IpBan, bool MachineBan)
        {
            if (string.IsNullOrEmpty(Reason))
                Reason = "No respetar las reglas del hotel";

            string Variable = Client.GetHabbo().Username.ToLower();
            string str = "user";
            double Expire = AkiledEnvironment.GetUnixTimestamp() + LengthSeconds;
            if (IpBan)
            {
                //Variable = Client.GetConnection().getIp();
                Variable = Client.GetHabbo().IP;
                str = "ip";
            }

            if (MachineBan)
            {
                Variable = Client.MachineId;
                str = "machine";
            }

            using (IQueryAdapter queryreactor = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                queryreactor.SetQuery("INSERT INTO bans (bantype,value,reason,expire,added_by,added_date) VALUES (@rawvar, @var, @reason, '" + Expire + "', @mod, UNIX_TIMESTAMP())");
                queryreactor.AddParameter("rawvar", str);
                queryreactor.AddParameter("var", Variable);
                queryreactor.AddParameter("reason", Reason);
                queryreactor.AddParameter("mod", Moderator);
                queryreactor.RunQuery();
            }
            if (MachineBan)
                BanUserAsync(Client, Moderator, LengthSeconds, Reason, true, false);
            else if (IpBan)
            {
                BanUserAsync(Client, Moderator, LengthSeconds, Reason, false, false);
            }
            else
            {
                Client.Disconnect();

            }

            return Task.CompletedTask;
        }

        public void SendSuperNotif(string Title, string Notice, string Picture, string Link, string LinkTitle, bool Broadcast, bool Event) => this.SendMessage(new RoomNotificationComposer(Title, Notice, Picture, LinkTitle, Link));

        public void SendWhisper(string Message, int Colour = 0)
        {
            try
            {
                foreach (GameClient Client in this._clients.Values.ToList())
                {
                    RoomUser User = Client.GetHabbo().CurrentRoom.GetRoomUserManager().GetRoomUserByHabbo(Client.GetHabbo().Username);
                    if (User == null || User.IsBot || User.IsPet)
                        continue;

                    SendMessage(new WhisperMessageComposer(User.VirtualId, Message, 0, Colour == 0 ? User.LastBubble : Colour));
                }
            }
            catch (Exception e)
            {
                Logging.HandleException(e, "GameClientManager.SendWhisper");
            }
        }

        private void GiveDiamonds()
        {
            if (diamondsQueuee.Count > 0)
            {
                lock (diamondsQueuee.SyncRoot)
                {
                    while (diamondsQueuee.Count > 0)
                    {
                        int amount = (int)diamondsQueuee.Dequeue();
                        foreach (GameClient client in _clients.Values)
                        {
                            if (client == null || client.GetHabbo() == null)
                                continue;

                            client.SendPacket(new HabboActivityPointNotificationComposer(client.GetHabbo().AkiledPoints, amount, 105));

                        }
                    }
                }
            }
        }

        private void GiveMoedas()
        {
            if (moedasQueuee.Count > 0)
            {
                lock (moedasQueuee.SyncRoot)
                {
                    while (moedasQueuee.Count > 0)
                    {
                        int amount = (int)moedasQueuee.Dequeue();
                        foreach (GameClient client in _clients.Values)
                        {
                            if (client == null || client.GetHabbo() == null)
                                continue;

                            client.SendPacket(new CreditBalanceComposer(client.GetHabbo().Credits));
                        }
                    }
                }
            }
        }

        public ICollection<GameClient> GetClients
        {
            get
            {
                return this._clients.Values;
            }
        }
    }
}
