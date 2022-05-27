using Akiled.HabboHotel.GameClients;

namespace Akiled.Communication.Packets.Incoming.Structure
{
    class SSOTicketEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {            if (Session == null || Session.GetHabbo() != null )
                return;            Session.TryAuthenticate(Packet.PopString());
        }
    }
}