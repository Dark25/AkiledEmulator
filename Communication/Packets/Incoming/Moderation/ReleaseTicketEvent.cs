using Akiled.HabboHotel.GameClients;

namespace Akiled.Communication.Packets.Incoming.Structure
{
    class ReleaseTicketEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            if (!Session.GetHabbo().HasFuse("fuse_mod"))
                return;
            int num = Packet.PopInt();
            for (int index = 0; index < num; ++index)
                AkiledEnvironment.GetGame().GetModerationManager().ReleaseTicket(Session, Packet.PopInt());

        }
    }
}
