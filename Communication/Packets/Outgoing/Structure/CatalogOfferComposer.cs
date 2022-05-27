using Akiled.HabboHotel.Catalog;
using Akiled.HabboHotel.Catalog.Utilities;
using Akiled.HabboHotel.Items;

namespace Akiled.Communication.Packets.Outgoing.Structure
{
    class CatalogOfferComposer : ServerPacket
    {
        public CatalogOfferComposer(CatalogItem Item)
            : base(ServerPacketHeader.CatalogOfferMessageComposer)
        {
            WriteInteger(Item.Id);
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

            WriteInteger(string.IsNullOrEmpty(Item.Badge) ? 1 : 2);

            if (!string.IsNullOrEmpty(Item.Badge))
            {
                WriteString("b");
                WriteString(Item.Badge);
            }

            WriteString(Item.Data.Type.ToString());
            if (Item.Data.Type.ToString().ToLower() == "b")
                base.WriteString(Item.Data.ItemName);//Badge name.
            else
            {
                WriteInteger(Item.Data.SpriteId);
                if (Item.Data.InteractionType == InteractionType.WALLPAPER || Item.Data.InteractionType == InteractionType.FLOOR || Item.Data.InteractionType == InteractionType.LANDSCAPE)
                {
                    WriteString(Item.Name.Split('_')[2]);
                }
                else if (Item.Data.InteractionType == InteractionType.bot)//Bots
                {
                    CatalogBot CatalogBot = null;
                    if (!AkiledEnvironment.GetGame().GetCatalog().TryGetBot(Item.ItemId, out CatalogBot))
                        WriteString("hd-180-7.ea-1406-62.ch-210-1321.hr-831-49.ca-1813-62.sh-295-1321.lg-285-92");
                    else
                        WriteString(CatalogBot.Figure);
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
            WriteInteger(0); //club_level
            WriteBoolean(ItemUtility.CanSelectAmount(Item));

            WriteBoolean(false);// TODO: Figure out
            WriteString("");//previewImage -> e.g; catalogue/pet_lion.png
        }
    }
}