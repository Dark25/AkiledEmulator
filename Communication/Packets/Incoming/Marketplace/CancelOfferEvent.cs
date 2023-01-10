using Akiled.Communication.Packets.Outgoing.Structure;
using Akiled.Database.Interfaces;
using Akiled.HabboHotel.Items;
using System;
using System.Data;

namespace Akiled.Communication.Packets.Incoming.Marketplace
{
    class CancelOfferEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            if (Session == null || Session.GetHabbo() == null)
                return;

            DataRow Row = null;
            int OfferId = Packet.PopInt();

            using (IQueryAdapter dbClient = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT `furni_id`, `item_id`, `user_id`, `extra_data`, `offer_id`, `state`, `timestamp`, `limited_number`, `limited_stack` FROM `catalog_marketplace_offers` WHERE `offer_id` = @OfferId LIMIT 1");
                dbClient.AddParameter("OfferId", OfferId);
                Row = dbClient.GetRow();
            }

            if (Row == null)
            {
                Session.SendPacket(new MarketplaceCancelOfferResultComposer(OfferId, false));
                return;
            }

            if (Convert.ToString(Row["state"]) == "2")
            {
                Session.SendPacket(new MarketplaceCancelOfferResultComposer(OfferId, false));
                return;
            }

            if (Convert.ToInt32(Row["user_id"]) != Session.GetHabbo().Id)
            {
                Session.SendPacket(new MarketplaceCancelOfferResultComposer(OfferId, false));
                return;
            }

            ItemData Item = null;
            if (!AkiledEnvironment.GetGame().GetItemManager().GetItem(Convert.ToInt32(Row["item_id"]), out Item))
            {
                Session.SendPacket(new MarketplaceCancelOfferResultComposer(OfferId, false));
                return;
            }

            using (IQueryAdapter dbClient = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("DELETE FROM `catalog_marketplace_offers` WHERE `offer_id` = @OfferId AND `user_id` = @UserId LIMIT 1");
                dbClient.AddParameter("OfferId", OfferId);
                dbClient.AddParameter("UserId", Session.GetHabbo().Id);
                dbClient.RunQuery();
            }

            Item GiveItem = ItemFactory.CreateSingleItem(Item, Session.GetHabbo(), Convert.ToString(Row["extra_data"]), Convert.ToInt32(Row["furni_id"]), Convert.ToInt32(Row["limited_number"]), Convert.ToInt32(Row["limited_stack"]));

            if (GiveItem != null)
            {
                Session.GetHabbo().GetInventoryComponent().TryAddItem(GiveItem);
                Session.SendPacket(new FurniListNotificationComposer(GiveItem.Id, 1));
            }
            Session.SendPacket(new MarketplaceCancelOfferResultComposer(OfferId, true));
        }
    }
}
