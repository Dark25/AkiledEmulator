using Akiled.Communication.Packets.Outgoing;
using Akiled.Database.Interfaces;
using Akiled.HabboHotel.GameClients;
using Akiled.HabboHotel.Items;
using Akiled.HabboHotel.Rooms.Wired.WiredHandlers.Interfaces;
using System.Collections.Generic;
using System.Data;

namespace Akiled.HabboHotel.Rooms.Wired.WiredHandlers.Triggers
{
    public class WalksOnFurni : IWired, IWiredCycleable
    {
        private Item item;
        private WiredHandler handler;
        private List<Item> items;
        private readonly UserAndItemDelegate delegateFunction;
        public int Delay { get; set; }
        private bool disposed;

        public WalksOnFurni(Item item, WiredHandler handler, List<Item> targetItems, int requiredCycles)
        {
            this.item = item;
            this.handler = handler;
            this.items = targetItems;
            this.delegateFunction = new UserAndItemDelegate(this.targetItem_OnUserWalksOnFurni);
            this.Delay = requiredCycles;
            foreach (Item roomItem in targetItems)
                roomItem.OnUserWalksOnFurni += this.delegateFunction;
            this.disposed = false;
        }

        public bool OnCycle(RoomUser User, Item Item)
        {
            if (User != null)
                this.handler.ExecutePile(this.item.Coordinate, User, Item);
            return false;
        }

        private void targetItem_OnUserWalksOnFurni(RoomUser user, Item item)
        {
            if (this.Delay > 0)
            {
                this.handler.RequestCycle(new WiredCycle(this, user, item, this.Delay));
            }
            else
            {
                this.handler.ExecutePile(this.item.Coordinate, user, item);
            }
        }

        public void Dispose()
        {
            this.disposed = true;
            if (this.items != null)
            {
                foreach (Item roomItem in this.items)
                    roomItem.OnUserWalksOnFurni -= this.delegateFunction;
                this.items.Clear();
            }
            this.items = (List<Item>)null;
            this.item = (Item)null;
            this.handler = (WiredHandler)null;
        }

        public void SaveToDatabase(IQueryAdapter dbClient)
        {
            WiredUtillity.SaveTriggerItem(dbClient, this.item.Id, string.Empty, this.Delay.ToString(), false, this.items);
        }

        public void LoadFromDatabase(IQueryAdapter dbClient, Room insideRoom)
        {
            dbClient.SetQuery("SELECT trigger_data, triggers_item FROM wired_items WHERE trigger_id = @id ");
            dbClient.AddParameter("id", this.item.Id);
            DataRow row = dbClient.GetRow();
            if (row == null)
                return;

            int OutDelay = 0;
            int.TryParse(row["trigger_data"].ToString(), out OutDelay);

            this.Delay = OutDelay;

            string itemslist = row["triggers_item"].ToString();

            if (itemslist == "")
                return;

            foreach (string id in itemslist.Split(';'))
            {
                int itemId = 0;
                int.TryParse(id, out itemId);

                if (itemId == 0)
                    continue;

                Item roomItem = insideRoom.GetRoomItemHandler().GetItem(itemId);
                if (roomItem != null && !this.items.Contains(roomItem) && roomItem.Id != this.item.Id)
                {
                    roomItem.OnUserWalksOnFurni += new UserAndItemDelegate(this.targetItem_OnUserWalksOnFurni);
                    this.items.Add(roomItem);
                }
            }
        }

        public void OnTrigger(GameClient Session, int SpriteId)
        {
            ServerPacket Message9 = new ServerPacket(ServerPacketHeader.WiredEffectConfigMessageComposer);
            Message9.WriteBoolean(false);
            Message9.WriteInteger(10);
            Message9.WriteInteger(this.items.Count);
            foreach (Item roomItem in this.items)
                Message9.WriteInteger(roomItem.Id);
            Message9.WriteInteger(SpriteId);
            Message9.WriteInteger(this.item.Id);
            Message9.WriteString("");
            Message9.WriteInteger(0);
            Message9.WriteInteger(8);
            Message9.WriteInteger(0);
            Message9.WriteInteger(this.Delay);
            Message9.WriteInteger(0);
            Message9.WriteInteger(0);
            Session.SendPacket(Message9);
        }

        public void DeleteFromDatabase(IQueryAdapter dbClient)
        {
            dbClient.RunQuery("DELETE FROM wired_items WHERE trigger_id = '" + this.item.Id + "'");
        }

        public bool Disposed()
        {
            return this.disposed;
        }
    }
}
