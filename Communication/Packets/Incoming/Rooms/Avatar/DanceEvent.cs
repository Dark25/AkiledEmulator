using Akiled.Communication.Packets.Outgoing;
using Akiled.HabboHotel.GameClients;
using Akiled.HabboHotel.Quests;
using Akiled.HabboHotel.Rooms;

namespace Akiled.Communication.Packets.Incoming.Structure
{
    class DanceEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            Room room = AkiledEnvironment.GetGame().GetRoomManager().GetRoom(Session.GetHabbo().CurrentRoomId);
            if (room == null)
                return;
            RoomUser roomUserByHabbo = room.GetRoomUserManager().GetRoomUserByHabboId(Session.GetHabbo().Id);
            if (roomUserByHabbo == null)
                return;
            roomUserByHabbo.Unidle();
            int i = Packet.PopInt();
            if (i < 0 || i > 4 || !true && i > 1)
                i = 0;
            if (i > 0 && roomUserByHabbo.CarryItemID > 0)
                roomUserByHabbo.CarryItem(0);
            roomUserByHabbo.DanceId = i;
            ServerPacket Message = new ServerPacket(ServerPacketHeader.DanceMessageComposer);
            Message.WriteInteger(roomUserByHabbo.VirtualId);
            Message.WriteInteger(i);
            room.SendPacket(Message);
            AkiledEnvironment.GetGame().GetQuestManager().ProgressUserQuest(Session, QuestType.SOCIAL_DANCE, 0);
        }
    }
}