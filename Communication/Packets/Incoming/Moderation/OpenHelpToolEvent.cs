using Akiled.Communication.Packets.Outgoing;
using Akiled.HabboHotel.GameClients;

namespace Akiled.Communication.Packets.Incoming.Structure
{
    class OpenHelpToolEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            ServerPacket serverMessage = new ServerPacket(ServerPacketHeader.OpenHelpToolMessageComposer);
            serverMessage.WriteInteger(0);
            Session.SendPacket(serverMessage);
        }
    }
}
