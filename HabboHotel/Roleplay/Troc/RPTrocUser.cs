using System.Collections.Generic;

namespace Akiled.HabboHotel.Roleplay.Troc
{
    public class RPTrocUser
    {
        public int UserId;
        public Dictionary<int, int> ItemIds;
        public bool Accepted;
        public bool Confirmed;

        public RPTrocUser(int pUserId)
        {
            this.UserId = pUserId;
            this.ItemIds = new Dictionary<int, int>();
            this.Accepted = false;
            this.Confirmed = false;
        }

        public int GetCountItem(int ItemId)
        {
            if (this.ItemIds.ContainsKey(ItemId))
                return this.ItemIds[ItemId];
            else
            {
                return 0;
            }
        }

        public void AddItemId(int ItemId)
        {
            if (!this.ItemIds.ContainsKey(ItemId))
                this.ItemIds.Add(ItemId, 1);
            else
            {
                this.ItemIds[ItemId]++;
            }
        }

        public void RemoveItemId(int ItemId)
        {
            if (!this.ItemIds.ContainsKey(ItemId))
                return;

            if (this.ItemIds[ItemId] > 1)
                this.ItemIds[ItemId]--;
            else
                this.ItemIds.Remove(ItemId);
        }
    }
}
