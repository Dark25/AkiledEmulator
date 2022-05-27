using Akiled.Communication.Packets.Outgoing.WebSocket;
using Akiled.HabboHotel.WebClients;

namespace Akiled.Communication.Packets.Incoming.WebSocket
{
    class PingWebEvent : IPacketWebEvent
    {
        public void Parse(WebClient Session, ClientPacket Packet)
        {
            Session.SendPacket(new PongComposer());
        }
    }
}
