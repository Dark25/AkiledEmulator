using Akiled.HabboHotel.GameClients;
using Akiled.HabboHotel.Support;

namespace Akiled.Communication.Packets.Incoming.Structure
{
    class ModerationMuteEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            if (!Session.GetHabbo().HasFuse("fuse_no_kick"))
                return;
            ModerationManager.KickUser(Session, Packet.PopInt(), Packet.PopString(), false);

        }
    }
}
