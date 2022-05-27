using Akiled.HabboHotel.GameClients;

namespace Akiled.Communication.Packets.Incoming.Structure
{
    class CloseTicketEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            if (!Session.GetHabbo().HasFuse("fuse_mod")) return;

            int Result = Packet.PopInt();
            Packet.PopInt();

            AkiledEnvironment.GetGame().GetModerationManager().CloseTicket(Session, Packet.PopInt(), Result);
        }
    }
}
