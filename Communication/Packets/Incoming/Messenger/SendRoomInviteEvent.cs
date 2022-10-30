using Akiled.Communication.Packets.Outgoing;
using Akiled.Communication.Packets.Outgoing.Structure;
using Akiled.Database.Interfaces;
using Akiled.HabboHotel.GameClients;
using Akiled.Utilities;
using System;
using System.Collections.Generic;

namespace Akiled.Communication.Packets.Incoming.Structure
{
    class SendRoomInviteEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            TimeSpan timeSpan = DateTime.Now - Session.GetHabbo().FloodTime;
            if (timeSpan.Seconds > 4)
                Session.GetHabbo().FloodCount = 0;
            if (timeSpan.Seconds < 4 && Session.GetHabbo().FloodCount > 5 && Session.GetHabbo().Rank < 5U)
            {
                return;
            }

            Session.GetHabbo().FloodTime = DateTime.Now;
            Session.GetHabbo().FloodCount++;

            int InviteCount = Packet.PopInt();
            if (InviteCount > 200)
                return;

            List<int> Targets = new List<int>();
            for (int i = 0; i < InviteCount; ++i)
            {
                int Id = Packet.PopInt();
                if (i < 100)
                    Targets.Add(Id);
            }

            string TextMessage = StringCharFilter.Escape(Packet.PopString());
            if (TextMessage.Length > 121)
                TextMessage = TextMessage.Substring(0, 121);

            if (Session.Antipub(TextMessage, "<RM>"))
            {
                AkiledEnvironment.GetGame().GetClientManager().StaffAlert(RoomNotificationComposer.SendBubble("publicidad", "El usuario: " + Session.GetHabbo().Username + ", Pub en invitación:" + TextMessage + ", pulsa aquí para ir a mirar.", "event:navigator/goto/" + Session.GetHabbo().CurrentRoomId));
                return;
            }
            if (Session.GetHabbo().IgnoreAll)
                return;

            ServerPacket Message = new ServerPacket(ServerPacketHeader.RoomInviteMessageComposer);
            Message.WriteInteger(Session.GetHabbo().Id);
            Message.WriteString(TextMessage);

            foreach (int UserId in Targets)
            {
                if (Session.GetHabbo().GetMessenger().FriendshipExists(UserId))
                {
                    GameClient clientByUserId = AkiledEnvironment.GetGame().GetClientManager().GetClientByUserID(UserId);
                    if (clientByUserId == null)
                        break;
                    if (clientByUserId.GetHabbo().GetMessenger().FriendshipExists(Session.GetHabbo().Id))
                        clientByUserId.SendPacket(Message);
                }
            }

            using (IQueryAdapter dbClient = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("INSERT INTO `chatlogs_console_invitations` (`user_id`,`message`,`timestamp`) VALUES ('" + Session.GetHabbo().Id + "', @message, UNIX_TIMESTAMP())");
                dbClient.AddParameter("message", Message);
                dbClient.RunQuery();
            }
        }
    }
}
