namespace Akiled.Communication.Packets.Outgoing.Structure
{
    class MarketplaceItemStatsComposer : ServerPacket
    {
        public MarketplaceItemStatsComposer(int ItemId, int SpriteId, int AveragePrice)
            : base(ServerPacketHeader.MarketplaceItemStatsMessageComposer)
        {
            WriteInteger(AveragePrice);//Avg price in last 7 days.
            WriteInteger(AkiledEnvironment.GetGame().GetCatalog().GetMarketplace().OfferCountForSprite(SpriteId));
            WriteInteger(7);//Day

            WriteInteger(4);//Count
            {
                WriteInteger(1); // Jour ?
                WriteInteger(2); // Prix moyen
                WriteInteger(1); // Volume échange

                WriteInteger(1); //x
                WriteInteger(2); //?
                WriteInteger(2); //y

                WriteInteger(3); //x
                WriteInteger(5);
                WriteInteger(3); //y

                WriteInteger(1); //x
                WriteInteger(7); //?
                WriteInteger(4); //y
            }

            WriteInteger(ItemId);
            WriteInteger(SpriteId);
        }
    }
}
