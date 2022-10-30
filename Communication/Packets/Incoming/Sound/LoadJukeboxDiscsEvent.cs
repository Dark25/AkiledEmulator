using Akiled.Communication.Packets.Outgoing;
using Akiled.Communication.Packets.Outgoing.Structure;
using Akiled.HabboHotel.GameClients;

namespace Akiled.Communication.Packets.Incoming.Sound
{
    internal class LoadJukeboxDiscsEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            if (Session.GetHabbo().CurrentRoom == null)
                return;
            Session.SendMessage((IServerPacket)new LoadJukeboxUserMusicItemsComposer(Session.GetHabbo().CurrentRoom));
        }
    }
}
