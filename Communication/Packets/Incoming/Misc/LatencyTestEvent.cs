using Akiled.Communication.Packets.Outgoing;
using Akiled.HabboHotel.GameClients;

namespace Akiled.Communication.Packets.Incoming.Structure
{
    class LatencyTestEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            ServerPacket Response = new ServerPacket(ServerPacketHeader.LatencyResponseMessageComposer);
            Response.WriteInteger(Packet.PopInt());
            Session.SendPacket(Response);
        }
    }
}