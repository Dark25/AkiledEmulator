using Akiled.Communication.Packets.Incoming;
using Akiled.HabboHotel.GameClients;


namespace Akiled.Communication.Packets
{

    public interface IPacketEvent
    {
        void Parse(GameClient session, ClientPacket packet);
    }
}
