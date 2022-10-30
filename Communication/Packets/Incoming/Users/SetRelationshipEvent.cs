using Akiled.Database.Interfaces;using Akiled.HabboHotel.GameClients;using Akiled.HabboHotel.Users.Messenger;using System;namespace Akiled.Communication.Packets.Incoming.Structure{    class SetRelationshipEvent : IPacketEvent    {        public void Parse(GameClient Session, ClientPacket Packet)        {            if (Session.GetHabbo() == null || Session.GetHabbo().GetMessenger() == null)                return;            int User = Packet.PopInt();            int Type = Packet.PopInt();            if (Type < 0 || Type > 3)                return;            if (!Session.GetHabbo().GetMessenger().FriendshipExists(Convert.ToInt32(User)))                return;            if (Type == 0)            {                if (Session.GetHabbo().GetMessenger().relation.ContainsKey(User))                {                    Session.GetHabbo().GetMessenger().relation.Remove(User);                }            }            else            {                if (Session.GetHabbo().GetMessenger().relation.ContainsKey(User))                {                    Session.GetHabbo().GetMessenger().relation[User].Type = Type;                }                else                {                    Session.GetHabbo().GetMessenger().relation.Add(User, new Relationship(User, Type));                }            }            using (IQueryAdapter dbClient = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())            {                dbClient.SetQuery("UPDATE messenger_friendships SET relation = '" + Type + "' WHERE user_one_id=@id AND user_two_id=@target LIMIT 1");                dbClient.AddParameter("id", Session.GetHabbo().Id);                dbClient.AddParameter("target", User);                dbClient.RunQuery();            }            GameClient Client = AkiledEnvironment.GetGame().GetClientManager().GetClientByUserID(Convert.ToInt32(User));            Session.GetHabbo().GetMessenger().RelationChanged(Convert.ToInt32(User), Type);            Session.GetHabbo().GetMessenger().UpdateFriend(Convert.ToInt32(User), true);




















































































            /*int User = Request.PopInt();            int Type = Request.PopInt();            using (IQueryAdapter dbClient = AkiledEnvironment.GetDatabaseManager().getQueryreactor())            {                if (Type == 0)                {                    if (Session.GetHabbo().GetMessenger().relation.ContainsKey(User))                    {                        Session.GetHabbo().GetMessenger().relation.Remove(User);                    }                }                else                {                    if (Session.GetHabbo().GetMessenger().relation.ContainsKey(User))                    {                        user_relationships SET target = '1'                        dbClient.setQuery("UPDATE user_relationships SET target = '" + Type + "' WHERE user_id=@id AND target=@target LIMIT 1");                        dbClient.addParameter("id", Session.GetHabbo().Id);                        dbClient.addParameter("target", User);                        dbClient.runQuery();                        if (Session.GetHabbo().GetMessenger().relation.ContainsKey(User))                        {                            Session.GetHabbo().GetMessenger().relation.Remove(User);                        }                    }                    else                    {                        dbClient.setQuery("INSERT INTO user_relationships (user_id, target, type) VALUES (@id, @target, @type)");                        dbClient.addParameter("id", Session.GetHabbo().Id);                        dbClient.addParameter("target", User);                        dbClient.addParameter("type", Type);                        int newId = (int)dbClient.insertQuery();                    }                    Session.GetHabbo().GetMessenger().relation.Add(User, new Relationship(User, Type));                }                GameClient Client = AkiledEnvironment.GetGame().GetClientManager().GetClientByUserID(Convert.ToInt32(User));                Session.GetHabbo().GetMessenger().UpdateFriend(Convert.ToInt32(User), Client, true);            }*/

        }    }}