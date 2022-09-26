using Akiled.Communication.Packets.Outgoing.Catalog;
using Akiled.HabboHotel.GameClients;

namespace Akiled.Communication.Packets.Incoming.Catalog
{
    internal class GetClubGiftsEvent : IPacketEvent
    {
        public void Parse(GameClient session, ClientPacket packet)
        {
            session.SendPacket(new ClubGiftsComposer());
        }
    }
}