using Akiled.Communication.Packets.Outgoing.Structure;
using Akiled.HabboHotel.Catalog;
using Akiled.HabboHotel.GameClients;
using System;

namespace Akiled.Communication.Packets.Incoming.Structure
{
    class GetCatalogOfferEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            int OfferId = Packet.PopInt();
            if (!AkiledEnvironment.GetGame().GetCatalog().ItemOffers.ContainsKey(OfferId))
                return;

            int PageId = AkiledEnvironment.GetGame().GetCatalog().ItemOffers[OfferId];

            CatalogPage Page;
            if (!AkiledEnvironment.GetGame().GetCatalog().TryGetPage(PageId, out Page))
                return;

            if (!Page.Enabled || Page.MinimumRank > Session.GetHabbo().Rank)
                return;
            
            CatalogItem Item = null;
            if (!Page.ItemOffers.ContainsKey(OfferId))
                return;

            Item = (CatalogItem)Page.ItemOffers[OfferId];
           
            if (Item != null)
                Session.SendMessage(new CatalogOfferComposer(Item));
        }
    }
}