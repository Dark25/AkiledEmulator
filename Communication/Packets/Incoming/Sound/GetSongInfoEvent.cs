using Akiled.Communication.Packets.Outgoing;
using Akiled.Communication.Packets.Outgoing.Structure;
using Akiled.HabboHotel.GameClients;

namespace Akiled.Communication.Packets.Incoming.Sound
{
    internal class GetSongInfoEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet) => Session.SendPacket((IServerPacket)new TraxSongInfoComposer());
    }
}
