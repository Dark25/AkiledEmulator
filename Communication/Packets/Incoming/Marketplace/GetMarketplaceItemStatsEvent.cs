﻿using Akiled.Communication.Packets.Outgoing.Structure;
using Akiled.Database.Interfaces;
using System;
using System.Data;

namespace Akiled.Communication.Packets.Incoming.Marketplace
{
    class GetMarketplaceItemStatsEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            int ItemId = Packet.PopInt();
            int SpriteId = Packet.PopInt();

            DataRow Row = null;
            using (IQueryAdapter dbClient = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT `avgprice` FROM `catalog_marketplace_data` WHERE `sprite` = @SpriteId LIMIT 1");
                dbClient.AddParameter("SpriteId", SpriteId);
                Row = dbClient.GetRow();
            }

            Session.SendPacket(new MarketplaceItemStatsComposer(ItemId, SpriteId, (Row != null ? Convert.ToInt32(Row["avgprice"]) : 0)));
        }
    }
}
