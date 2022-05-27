using Akiled.HabboHotel.WebClients;

namespace Akiled.Communication.Packets.Incoming.WebSocket
{
    class SSOTicketWebEvent : IPacketWebEvent
    {
        public void Parse(WebClient Session, ClientPacket Packet)
        {            if (Session == null)                return;            Session.TryAuthenticate(Packet.PopString());
        }
    }
}
