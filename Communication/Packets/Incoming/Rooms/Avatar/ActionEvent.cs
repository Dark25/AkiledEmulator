using Akiled.Communication.Packets.Outgoing;
using Akiled.Communication.Packets.Outgoing.Structure;
using Akiled.HabboHotel.GameClients;
using Akiled.HabboHotel.Quests;
using Akiled.HabboHotel.Rooms;

namespace Akiled.Communication.Packets.Incoming.Structure
{
    class ActionEvent : IPacketEvent
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
            roomUserByHabbo.DanceId = 0;

            room.SendPacket(new ActionMessageComposer(roomUserByHabbo.VirtualId, i));
            if (i == 5)
            {
                roomUserByHabbo.IsAsleep = true;
                ServerPacket Message2 = new ServerPacket(ServerPacketHeader.SleepMessageComposer);
                Message2.WriteInteger(roomUserByHabbo.VirtualId);
                Message2.WriteBoolean(roomUserByHabbo.IsAsleep);
                room.SendPacket(Message2);
            }
            AkiledEnvironment.GetGame().GetQuestManager().ProgressUserQuest(Session, QuestType.SOCIAL_WAVE, 0);

        }
    }
}
