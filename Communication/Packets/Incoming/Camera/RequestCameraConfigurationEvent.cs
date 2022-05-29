using Akiled.Communication.Packets.Outgoing.Structure;
using Akiled.HabboHotel.GameClients;

namespace Akiled.Communication.Packets.Incoming.Structure
{
    class RequestCameraConfigurationEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet) => Session.SendPacket(new CameraPriceComposer(0, 0, 0));
    }
}
