using Akiled.Communication.Packets.Outgoing;
using Akiled.Communication.Packets.Outgoing.Structure;
using Akiled.HabboHotel.GameClients;
using Akiled.HabboHotel.Quests;
using Akiled.HabboHotel.Rooms;

namespace Akiled.Communication.Packets.Incoming.Structure
{
    class RespectUserEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            if (Session == null || Session.GetHabbo() == null)
                return;

            Room room = AkiledEnvironment.GetGame().GetRoomManager().GetRoom(Session.GetHabbo().CurrentRoomId);
            if (room == null || Session.GetHabbo().DailyRespectPoints <= 0)
                return;
            RoomUser roomUserByHabbo = room.GetRoomUserManager().GetRoomUserByHabboId(Packet.PopInt());
            if (roomUserByHabbo == null || roomUserByHabbo.GetClient().GetHabbo().Id == Session.GetHabbo().Id || roomUserByHabbo.IsBot)
                return;
            AkiledEnvironment.GetGame().GetQuestManager().ProgressUserQuest(Session, QuestType.SOCIAL_RESPECT, 0);
            AkiledEnvironment.GetGame().GetAchievementManager().ProgressAchievement(roomUserByHabbo.GetClient(), "ACH_RespectEarned", 1);
            AkiledEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_RespectGiven", 1);
            Session.GetHabbo().DailyRespectPoints--;
            roomUserByHabbo.GetClient().GetHabbo().Respect++;

            ServerPacket Message = new ServerPacket(ServerPacketHeader.RespectNotificationMessageComposer);
            Message.WriteInteger(roomUserByHabbo.GetClient().GetHabbo().Id);
            Message.WriteInteger(roomUserByHabbo.GetClient().GetHabbo().Respect);
            room.SendPacket(Message);

            RoomUser roomUserByHabbo2 = room.GetRoomUserManager().GetRoomUserByHabboId(Session.GetHabbo().Id);

            room.SendPacket(new ActionMessageComposer(roomUserByHabbo2.VirtualId, 7));
        }
    }
}