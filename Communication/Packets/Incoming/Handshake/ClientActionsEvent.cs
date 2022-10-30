using Akiled.HabboHotel.GameClients;

namespace Akiled.Communication.Packets.Incoming.Structure
{
    class ClientActionsEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            string Category = Packet.PopString();
            string Type = Packet.PopString();
            string Action = Packet.PopString();
            string ExtraString = Packet.PopString();
            int ExtraInt = Packet.PopInt();
        }
    }
}
