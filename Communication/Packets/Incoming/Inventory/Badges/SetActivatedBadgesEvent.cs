using Akiled.Communication.Packets.Outgoing;
using Akiled.Database.Interfaces;
using Akiled.HabboHotel.GameClients;
using Akiled.HabboHotel.Quests;
using Akiled.HabboHotel.Users.Badges;

namespace Akiled.Communication.Packets.Incoming.Structure
{
    class SetActivatedBadgesEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            Session.GetHabbo().GetBadgeComponent().ResetSlots();            using (IQueryAdapter queryreactor = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())                queryreactor.RunQuery("UPDATE user_badges SET badge_slot = '0' WHERE user_id = '" + Session.GetHabbo().Id + "' AND badge_slot != '0'");

            for (int i = 0; i < 8; i++)            {                int Slot = Packet.PopInt();                string Badge = Packet.PopString();                if (string.IsNullOrEmpty(Badge))                    continue;                if (!Session.GetHabbo().GetBadgeComponent().HasBadge(Badge) || Slot < 1 || Slot > 8)                    continue;                Session.GetHabbo().GetBadgeComponent().GetBadge(Badge).Slot = Slot;                using (IQueryAdapter queryreactor = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())                {                    queryreactor.SetQuery(string.Concat(new object[4] { "UPDATE user_badges SET badge_slot = ", Slot, " WHERE badge_id = @badge AND user_id = ", Session.GetHabbo().Id }));                    queryreactor.AddParameter("badge", Badge);                    queryreactor.RunQuery();                }            }            AkiledEnvironment.GetGame().GetQuestManager().ProgressUserQuest(Session, QuestType.PROFILE_BADGE, 0);            ServerPacket Message = new ServerPacket(ServerPacketHeader.HabboUserBadgesMessageComposer);            Message.WriteInteger(Session.GetHabbo().Id);            Message.WriteInteger(Session.GetHabbo().GetBadgeComponent().EquippedCount);            int BadgeCount = 0;            foreach (Badge badge in Session.GetHabbo().GetBadgeComponent().BadgeList.Values)            {                if (badge.Slot > 0)                {                    BadgeCount++;                    if (BadgeCount > 8)                        break;                    Message.WriteInteger(badge.Slot);                    Message.WriteString(badge.Code);                }            }            if (Session.GetHabbo().InRoom && AkiledEnvironment.GetGame().GetRoomManager().GetRoom(Session.GetHabbo().CurrentRoomId) != null)                AkiledEnvironment.GetGame().GetRoomManager().GetRoom(Session.GetHabbo().CurrentRoomId).SendPacket(Message);

            Session.SendPacket(Message);
        }
    }
}
