using Akiled.HabboHotel.GameClients;
using Akiled.HabboHotel.Support;

namespace Akiled.Communication.Packets.Incoming.Structure
{
    class ModerateRoomEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            if (!Session.GetHabbo().HasFuse("fuse_mod"))
                return;
            int RoomId = Packet.PopInt();
            bool LockRoom = Packet.PopInt() == 1;
            bool InappropriateRoom = Packet.PopInt() == 1;
            bool KickUsers = Packet.PopInt() == 1;
            ModerationManager.PerformRoomAction(Session, RoomId, KickUsers, LockRoom, InappropriateRoom);

        }
    }
}
