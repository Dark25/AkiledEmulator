using Akiled.HabboHotel.GameClients;

namespace Akiled.Communication.Packets.Incoming.Structure
{
    class PickTicketEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            if (!Session.GetHabbo().HasFuse("fuse_mod"))
                return;
            Packet.PopInt();
            AkiledEnvironment.GetGame().GetModerationManager().PickTicket(Session, Packet.PopInt());

        }
    }
}
