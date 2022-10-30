using Akiled.HabboHotel.GameClients;
using Akiled.HabboHotel.Support;

namespace Akiled.Communication.Packets.Incoming.Structure
{
    class GetModeratorUserChatlogEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            if (!Session.GetHabbo().HasFuse("fuse_chatlogs"))
                return;
            Session.SendPacket(ModerationManager.SerializeUserChatlog(Packet.PopInt(), Session.GetHabbo().CurrentRoomId));

        }
    }
}
