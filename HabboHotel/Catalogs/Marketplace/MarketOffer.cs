namespace Akiled.HabboHotel.Catalog.Marketplace
{
    public class MarketOffer
    {
        public int OfferID;
        public int ItemType;
        public int SpriteId;
        public int TotalPrice;
        public int LimitedNumber;
        public int LimitedStack;

        public MarketOffer(int OfferID, int SpriteId, int TotalPrice, int ItemType, int LimitedNumber, int LimitedStack)
        {
            this.OfferID = OfferID;
            this.SpriteId = SpriteId;
            this.ItemType = ItemType;
            this.TotalPrice = TotalPrice;
            this.LimitedNumber = LimitedNumber;
            this.LimitedStack = LimitedStack;
        }
    }
}
