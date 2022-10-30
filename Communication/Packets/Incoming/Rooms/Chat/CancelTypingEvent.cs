using Akiled.Communication.Packets.Outgoing;
using Akiled.HabboHotel.GameClients;
using Akiled.HabboHotel.Rooms;

namespace Akiled.Communication.Packets.Incoming.Structure
{
    class CancelTypingEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            Room room = AkiledEnvironment.GetGame().GetRoomManager().GetRoom(Session.GetHabbo().CurrentRoomId);
            if (room == null)
                return;
            RoomUser roomUserByHabbo = room.GetRoomUserManager().GetRoomUserByHabboId(Session.GetHabbo().Id);
            if (roomUserByHabbo == null)
                return;
            ServerPacket Message = new ServerPacket(ServerPacketHeader.UserTypingMessageComposer);
            Message.WriteInteger(roomUserByHabbo.VirtualId);
            Message.WriteInteger(0);
            room.SendPacket(Message);

        }
    }
}
