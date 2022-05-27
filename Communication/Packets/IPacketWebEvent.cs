using Akiled.Communication.Packets.Incoming;
using Akiled.HabboHotel.WebClients;

namespace Akiled.Communication.Packets
{
    public interface IPacketWebEvent
    {
        void Parse(WebClient session, ClientPacket packet);
    }
}
