using Akiled.HabboHotel.Catalog;
using Akiled.HabboHotel.Catalog.Utilities;
using Akiled.HabboHotel.Items;
using System;

namespace Akiled.Communication.Packets.Outgoing.Structure
{
    class CatalogOfferComposer : ServerPacket
    {
        public CatalogOfferComposer(CatalogItem Item)
            : base(ServerPacketHeader.CatalogOfferMessageComposer)
        {
            
            WriteInteger(Item.OfferId);
            WriteString(Item.Name);
            WriteBoolean(false);//IsRentable
            WriteInteger(Item.CostCredits);

            if (Item.CostDiamonds > 0)
            {
                WriteInteger(Item.CostDiamonds);
                WriteInteger(105); // Diamonds
            }
            else
            {
                WriteInteger(Item.CostDuckets);
                WriteInteger(0);
            }

            WriteBoolean(ItemUtility.CanGiftItem(Item));

            WriteInteger(string.IsNullOrEmpty(Item.Badge) || Item.Data.Type.ToString() == "b" ? 1 : 2);

            if (Item.Data.Type.ToString().ToLower() != "b")
            {
                WriteString(Item.Data.Type.ToString());
                WriteInteger(Item.Data.SpriteId);
                if (Item.Data.InteractionType is InteractionType.WALLPAPER or InteractionType.FLOOR or InteractionType.LANDSCAPE)
                {
                    WriteString(Item.Name.Split('_')[2]);
                }
                else if (Item.Data.InteractionType == InteractionType.bot)//Bots
                {
                    if (!AkiledEnvironment.GetGame().GetCatalog().TryGetBot(Item.ItemId, out var catalogBot))
                    {
                        WriteString("hd-180-7.ea-1406-62.ch-210-1321.hr-831-49.ca-1813-62.sh-295-1321.lg-285-92");
                    }
                    else
                    {
                        WriteString(catalogBot.Figure);
                    }
                }
                else
                {
                    WriteString("");
                }
                WriteInteger(Item.Amount);
                WriteBoolean(Item.IsLimited); // IsLimited
                if (Item.IsLimited)
                {
                    WriteInteger(Item.LimitedEditionStack);
                    WriteInteger(Item.LimitedEditionStack - Item.LimitedEditionSells);
                }
            }

            if (!string.IsNullOrEmpty(Item.Badge))
            {
                WriteString("b");
                WriteString(Item.Badge);
            }

            WriteInteger(0); //club_level
            WriteBoolean(ItemUtility.CanSelectAmount(Item));

            WriteBoolean(false);// TODO: Figure out
            WriteString("");//previewImage -> e.g; catalogue/pet_lion.png
        }
    }
}