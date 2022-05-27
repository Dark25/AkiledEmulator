using Akiled.Communication.Packets.Outgoing.Structure;
using Akiled.HabboHotel.GameClients;using System;namespace Akiled.Communication.Packets.Incoming.Structure{    class SendMsgEvent : IPacketEvent    {        public void Parse(GameClient Session, ClientPacket Packet)        {            if (Session == null || Session.GetHabbo() == null || Session.GetHabbo().GetMessenger() == null)
                return;

            int userId = Packet.PopInt();

            if (userId == Session.GetHabbo().Id)
                return;


            string Message = AkiledEnvironment.GetGame().GetChatManager().GetFilter().CheckMessage(Packet.PopString());
            if (string.IsNullOrWhiteSpace(Message))
                return;
            TimeSpan timeSpan = DateTime.Now - Session.GetHabbo().FloodTime;            if (timeSpan.Seconds > 2)                Session.GetHabbo().FloodCount = 0;            if (timeSpan.Seconds < 4 && Session.GetHabbo().FloodCount > 10 && Session.GetHabbo().Rank < 5)                return;

            Session.GetHabbo().FloodTime = DateTime.Now;
            Session.GetHabbo().FloodCount++;

            if (Session.Antipub(Message, "<MP>"))
            {
                AkiledEnvironment.GetGame().GetClientManager().StaffAlert(RoomNotificationComposer.SendBubble("publicidad", "El usuario: " + Session.GetHabbo().Username + ", palabra:" + Message + ", pulsa aquí para ir a mirar.", "event:navigator/goto/" + Session.GetHabbo().CurrentRoomId));
                return;
            }
            if (Session.GetHabbo().IgnoreAll)
                return;


            string staffchatrank = (AkiledEnvironment.GetConfig().data["staffchatrank"]);
            string modschatrank = (AkiledEnvironment.GetConfig().data["modschatrank"]);
            string gamemasterchatrank = (AkiledEnvironment.GetConfig().data["gamemasterchatrank"]);
            string interschatrank = (AkiledEnvironment.GetConfig().data["interschatrank"]);
            string guiaschatrank = (AkiledEnvironment.GetConfig().data["guiaschatrank"]);
            string pubschatrank = (AkiledEnvironment.GetConfig().data["pubschatrank"]);


            if (userId == 0x7fffffff && Session.GetHabbo().Rank >= Convert.ToInt32(staffchatrank))
            {

                AkiledEnvironment.GetGame().GetClientManager().Chatstaff(new NewConsoleMessageComposer(0x7fffffff, Session.GetHabbo().Username + ": " + Message), Session.GetHabbo().Id);
                return;
            }

            if (userId == 0x6fffffff && Session.GetHabbo().Rank >= Convert.ToInt32(modschatrank) && Session.GetHabbo().Ismod)
            {

                AkiledEnvironment.GetGame().GetClientManager().Chatstaffmods(new NewConsoleMessageComposer(0x6fffffff, Session.GetHabbo().Username + ": " + Message), Session.GetHabbo().Id);
                return;
            }

            if (userId == 0x5fffffff && Session.GetHabbo().Rank >= Convert.ToInt32(gamemasterchatrank) && Session.GetHabbo().Isgm)
            {

                AkiledEnvironment.GetGame().GetClientManager().Chatstaffgms(new NewConsoleMessageComposer(0x5fffffff, Session.GetHabbo().Username + ": " + Message), Session.GetHabbo().Id);
                return;
            }

            if (userId == 0x4fffffff && Session.GetHabbo().Rank >= Convert.ToInt32(interschatrank) && Session.GetHabbo().Isinter)
            {

                AkiledEnvironment.GetGame().GetClientManager().Chatstaffinter(new NewConsoleMessageComposer(0x4fffffff, Session.GetHabbo().Username + ": " + Message), Session.GetHabbo().Id);
                return;
            }

            if (userId == 0x3fffffff && Session.GetHabbo().Rank >= Convert.ToInt32(guiaschatrank) && Session.GetHabbo().Isguia)
            {

                AkiledEnvironment.GetGame().GetClientManager().Chatstaffguias(new NewConsoleMessageComposer(0x3fffffff, Session.GetHabbo().Username + ": " + Message), Session.GetHabbo().Id);
                return;
            }

            if (userId == 0x2fffffff && Session.GetHabbo().Rank >= Convert.ToInt32(pubschatrank) && Session.GetHabbo().Ispub)
            {

                AkiledEnvironment.GetGame().GetClientManager().Chatstaffpubs(new NewConsoleMessageComposer(0x2fffffff, Session.GetHabbo().Username + ": " + Message), Session.GetHabbo().Id);
                return;
            }

            Session.GetHabbo().GetMessenger().SendInstantMessage(userId, Message);
        }

    }
}