using Akiled.Communication.Packets.Outgoing.Structure;
using Akiled.Database.Interfaces;
using Akiled.HabboHotel.GameClients;
using Akiled.HabboHotel.Quests;
using Akiled.HabboHotel.Rooms;
using Akiled.Utilities;

namespace Akiled.Communication.Packets.Incoming.Structure
{
    class ChangeMottoEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            string newMotto = StringCharFilter.Escape(Packet.PopString());
            if (newMotto == Session.GetHabbo().Motto)
                return;
            if (newMotto.Length > 38)
                newMotto = newMotto.Substring(0, 38);

            if (Session.Antipub(newMotto, "<MOTTO>"))
            {
                AkiledEnvironment.GetGame().GetClientManager().StaffAlert(RoomNotificationComposer.SendBubble("publicidad", "El usuario: " + Session.GetHabbo().Username + ", Pub en cambio de misíon:" + newMotto + ", pulsa aquí para ir a mirar.", "event:navigator/goto/" + Session.GetHabbo().CurrentRoomId));
                return;
            }

            if (!Session.GetHabbo().HasFuse("word_filter_override"))
                newMotto = AkiledEnvironment.GetGame().GetChatManager().GetFilter().CheckMessage(newMotto);

            if (Session.GetHabbo().IgnoreAll)
                return;

            Session.GetHabbo().Motto = newMotto;
            using (IQueryAdapter queryreactor = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                queryreactor.SetQuery("UPDATE users SET motto = @motto WHERE id = '" + Session.GetHabbo().Id + "'");
                queryreactor.AddParameter("motto", newMotto);
                queryreactor.RunQuery();
            }
            AkiledEnvironment.GetGame().GetQuestManager().ProgressUserQuest(Session, QuestType.PROFILE_CHANGE_MOTTO, 0);
            if (Session.GetHabbo().InRoom)
            {
                Room currentRoom = Session.GetHabbo().CurrentRoom;
                if (currentRoom == null)
                    return;
                RoomUser roomUserByHabbo = currentRoom.GetRoomUserManager().GetRoomUserByHabboId(Session.GetHabbo().Id);
                if (roomUserByHabbo == null)
                    return;

                if (roomUserByHabbo.transformation || roomUserByHabbo.IsSpectator)
                    return;

                currentRoom.SendPacket(new UserChangeComposer(roomUserByHabbo, false));
            }
            AkiledEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_Motto", 1);
        }
    }
}