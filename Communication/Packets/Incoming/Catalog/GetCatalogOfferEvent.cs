using Akiled.Communication.Packets.Outgoing.Structure;
using Akiled.HabboHotel.Catalog;
using Akiled.HabboHotel.GameClients;

namespace Akiled.Communication.Packets.Incoming.Structure
{
    class GetCatalogOfferEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            int id = Packet.PopInt();
            CatalogItem Item = AkiledEnvironment.GetGame().GetCatalog().FindItem(id, Session.GetHabbo().Rank);
            if (Item == null)
                return;

            CatalogPage Page;
            if (!AkiledEnvironment.GetGame().GetCatalog().TryGetPage(Item.PageID, out Page))
                return;

            if (!Page.Enabled || Page.MinimumRank > Session.GetHabbo().Rank)
                return;

            if (Item.IsLimited)
                return;

            Session.SendPacket(new CatalogOfferComposer(Item));
        }
    }
}