using Akiled.HabboHotel.Items;

namespace Akiled.HabboHotel.Catalog
{
    public class CatalogItem
    {
        public int Id;
        public int ItemId;
        public ItemData Data;
        public int Amount;
        public int CostCredits;
        public bool HaveOffer;
        public bool IsLimited;
        public string Name;
        public int PageID;
        public int CostDuckets;
        public int LimitedEditionStack;
        public int LimitedEditionSells;
        public int CostDiamonds;
        public string Badge;
        public int OfferId { get; set; }

        public CatalogItem(int Id, int ItemId, ItemData Data, string CatalogName, int PageId, int CostCredits, int CostPixels,
            int CostDiamonds, int Amount, int LimitedEditionSells, int LimitedEditionStack, bool HaveOffer, string badge, int OfferId)
        {
            this.Id = Id;
            this.Name = CatalogName;
            this.ItemId = ItemId;
            this.Data = Data;
            this.PageID = PageId;
            this.CostCredits = CostCredits;
            this.CostDuckets = CostPixels;
            this.CostDiamonds = CostDiamonds;
            this.Amount = Amount;
            this.LimitedEditionSells = LimitedEditionSells;
            this.LimitedEditionStack = LimitedEditionStack;
            this.IsLimited = (LimitedEditionStack > 0);
            this.HaveOffer = HaveOffer;
            this.Badge = badge;
            this.OfferId = OfferId;
        }
    }
}