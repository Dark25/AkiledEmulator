using Akiled.HabboHotel.GameClients;
using Akiled.HabboHotel.Support;

namespace Akiled.Communication.Packets.Incoming.Structure
{
    class GetModeratorRoomChatlogEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            if (!Session.GetHabbo().HasFuse("fuse_chatlogs"))                return;            Packet.PopInt();            int roomID = Packet.PopInt();            if (AkiledEnvironment.GetGame().GetRoomManager().GetRoom(roomID) == null)                return;            Session.SendPacket(ModerationManager.SerializeRoomChatlog(roomID));
        }
    }
}
