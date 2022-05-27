using Akiled.Communication.Packets.Incoming;
using Akiled.HabboHotel.GameClients;
using Akiled.Utilities.DependencyInjection;


namespace Akiled.Communication.Packets
{
    [Transient]
    public interface IPacketEvent
    {
        void Parse(GameClient session, ClientPacket packet);
    }
}
