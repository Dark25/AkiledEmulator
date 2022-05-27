﻿using Akiled.HabboHotel.Catalog.Marketplace;
using System.Collections.Generic;

namespace Akiled.Communication.Packets.Outgoing.Structure
{
    class MarketPlaceOffersComposer : ServerPacket
    {
        public MarketPlaceOffersComposer(int MinCost, int MaxCost, Dictionary<int, MarketOffer> dictionary, Dictionary<int, int> dictionary2)
            : base(ServerPacketHeader.MarketPlaceOffersMessageComposer)
        {
            WriteInteger(dictionary.Count);
            if (dictionary.Count > 0)
            {
                foreach (KeyValuePair<int, MarketOffer> pair in dictionary)
                {
                    WriteInteger(pair.Value.OfferID);
                    WriteInteger(1);//State
                    WriteInteger(1);
                    WriteInteger(pair.Value.SpriteId);

                    WriteInteger(256);
                    WriteString("");
                    WriteInteger(pair.Value.LimitedNumber);
                    WriteInteger(pair.Value.LimitedStack);

                    WriteInteger(pair.Value.TotalPrice);
                    WriteInteger(0);
                    WriteInteger(AkiledEnvironment.GetGame().GetCatalog().GetMarketplace().AvgPriceForSprite(pair.Value.SpriteId));
                    WriteInteger(dictionary2[pair.Value.SpriteId]);
                }
            }
            WriteInteger(dictionary.Count);//Item count to show how many were found.
        }
    }
}
