using Akiled.Communication.Packets.Outgoing;
using Akiled.HabboHotel.GameClients;
using Akiled.HabboHotel.Rooms;
using Akiled.HabboHotel.Users;

namespace Akiled.Communication.Packets.Incoming.Structure
{
    class WhiperGroupEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            Room room = AkiledEnvironment.GetGame().GetRoomManager().GetRoom(Session.GetHabbo().CurrentRoomId);
            if (room == null)
                return;

            RoomUser roomUserByHabbo = room.GetRoomUserManager().GetRoomUserByHabboId(Session.GetHabbo().Id);
            if (roomUserByHabbo == null)
                return;

            string name = Packet.PopString();

            RoomUser roomOtherUserByHabbo = room.GetRoomUserManager().GetRoomUserByHabbo(name);
            if (roomOtherUserByHabbo == null)
                return;

            if (!roomUserByHabbo.WhiperGroupUsers.Contains(roomOtherUserByHabbo.GetUsername()))
            {
                if (roomUserByHabbo.WhiperGroupUsers.Count >= 5)
                {
                    return;
                }

                roomUserByHabbo.WhiperGroupUsers.Add(roomOtherUserByHabbo.GetUsername());
            }
            else
                roomUserByHabbo.WhiperGroupUsers.Remove(roomOtherUserByHabbo.GetUsername());
        }
    }
}