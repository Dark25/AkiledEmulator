namespace Akiled.HabboHotel.Roleplay.Player
{
    public class RolePlayInventoryItem
    {
        public int Id;
        public int ItemId;
        public int Count;

        public RolePlayInventoryItem(int Id, int ItemId, int Count)
        {
            this.Id = Id;
            this.ItemId = ItemId;
            this.Count = Count;
        }
    }
}
