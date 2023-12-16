using Akiled;
using Akiled.Communication.Packets.Outgoing;
using Akiled.HabboHotel.Catalog;
using Akiled.HabboHotel.Catalog.Utilities;
using Akiled.HabboHotel.Items;

namespace AkiledEmulator.HabboHotel.Catalogs
{
    internal static class CatalogItemUtility
    {
        public static void GenerateOfferData(CatalogItem item, bool isPremium, ServerPacket message)
        {
            message.WriteInteger(item.Id);
            message.WriteString(item.Name);
            message.WriteBoolean(false);//IsRentable
            message.WriteInteger(item.CostCredits);

            if (item.CostDiamonds > 0)
            {
                message.WriteInteger(item.CostDiamonds);
                message.WriteInteger(105);
            }
            else
            {
                message.WriteInteger(item.CostDuckets);
                message.WriteInteger(0);
            }

            message.WriteBoolean(ItemUtility.CanGiftItem(item));

            message.WriteInteger(string.IsNullOrEmpty(item.Badge) || item.Data.Type.ToString() == "b" ? 1 : 2);

            if (item.Data.Type.ToString().ToLower() != "b")
            {
                message.WriteString(item.Data.Type.ToString());
                message.WriteInteger(item.Data.SpriteId);
                if (item.Data.InteractionType is InteractionType.WALLPAPER or InteractionType.FLOOR or InteractionType.LANDSCAPE)
                {
                    message.WriteString(item.Name.Split('_')[2]);
                }
                else if (item.Data.InteractionType == InteractionType.bot)//Bots
                {
                    if (!AkiledEnvironment.GetGame().GetCatalog().TryGetBot(item.ItemId, out var catalogBot))
                    {
                        message.WriteString("hd-180-7.ea-1406-62.ch-210-1321.hr-831-49.ca-1813-62.sh-295-1321.lg-285-92");
                    }
                    else
                    {
                        message.WriteString(catalogBot.Figure);
                    }
                }
                else
                {
                    message.WriteString("");
                }
                message.WriteInteger(item.Amount);
                message.WriteBoolean(item.IsLimited); // IsLimited
                if (item.IsLimited)
                {
                    message.WriteInteger(item.LimitedEditionStack);
                    message.WriteInteger(item.LimitedEditionStack - item.LimitedEditionSells);
                }
            }

            if (!string.IsNullOrEmpty(item.Badge))
            {
                message.WriteString("b");
                message.WriteString(item.Badge);
            }

            message.WriteInteger(isPremium ? 2 : 0); //club_level
            message.WriteBoolean(ItemUtility.CanSelectAmount(item));

            message.WriteBoolean(false);// TODO: Figure out
            message.WriteString("");//previewImage -> e.g; catalogue/pet_lion.png
        }
    }
}
