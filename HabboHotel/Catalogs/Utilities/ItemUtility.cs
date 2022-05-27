﻿using Akiled.HabboHotel.Items;

namespace Akiled.HabboHotel.Catalog.Utilities
{
    public static class ItemUtility
    {
        public static bool CanGiftItem(CatalogItem Item)
        {
            if (Item.Data.InteractionType == InteractionType.TROPHY) 
                return true;

            if (!Item.Data.AllowGift || Item.IsLimited || Item.Amount > 1 || Item.Data.InteractionType == InteractionType.EXCHANGE ||
                Item.Data.InteractionType == InteractionType.BADGE || (Item.Data.Type != 's' && Item.Data.Type != 'i') || Item.CostDiamonds > 0 ||
                Item.Data.InteractionType == InteractionType.TELEPORT || Item.Data.InteractionType == InteractionType.ARROW)
                return false;

            if (Item.Data.IsRare)
                return false;

            if (Item.Data.InteractionType == InteractionType.pet)
                return false;

            return true;
        }

        public static bool CanSelectAmount(CatalogItem Item)
        {
            if (Item.IsLimited || Item.Amount > 1 || Item.Data.InteractionType == InteractionType.EXCHANGE || !Item.HaveOffer || Item.Data.InteractionType == InteractionType.BADGE)
                return false;
            return true;
        }

        public static int GetSaddleId(int Saddle)
        {
            switch (Saddle)
            {
                default:
                case 9:
                    return 2804;
                case 10:
                    return 7544143;
            }
        }

        public static bool IsRare(Item Item)
        {
            if (Item.LimitedNo > 0)
                return true;

            if (Item.Data.IsRare)
                return true;

            return false;
        }
    }
}