using Akiled.Communication.Packets.Outgoing;
using Akiled.HabboHotel.GameClients;

namespace Akiled.Communication.Packets.Incoming.Structure
{
    class RecomendHelpers : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {            ServerPacket Response = new ServerPacket(ServerPacketHeader.OnGuideSessionDetached);            Session.SendPacket(Response);
        }
    }
}
