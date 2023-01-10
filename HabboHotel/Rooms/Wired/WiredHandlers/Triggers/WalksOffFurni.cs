using Akiled.Communication.Packets.Outgoing;
using Akiled.Database.Interfaces;
using Akiled.HabboHotel.GameClients;
using Akiled.HabboHotel.Items;
using Akiled.HabboHotel.Rooms.Wired.WiredHandlers.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;

namespace Akiled.HabboHotel.Rooms.Wired.WiredHandlers.Triggers
{
    public class WalksOffFurni : IWired, IWiredCycleable
    {
        private Item item;
        private WiredHandler handler;
        private List<Item> items;
        private readonly UserAndItemDelegate delegateFunction;
        public int Delay { get; set; }
        private bool disposed;

        public WalksOffFurni(Item item, WiredHandler handler, List<Item> targetItems, int requiredCycles)
        {
            this.item = item;
            this.handler = handler;
            this.items = targetItems;
            this.delegateFunction = new UserAndItemDelegate(this.targetItem_OnUserWalksOffFurni);
            this.Delay = requiredCycles;
            foreach (Item roomItem in targetItems)
                roomItem.OnUserWalksOffFurni += this.delegateFunction;

            this.disposed = false;
        }

        public bool OnCycle(RoomUser user, Item item)
        {
            if (user != null)
                this.handler.ExecutePile(this.item.Coordinate, user, item);
            return false;
        }

        private void targetItem_OnUserWalksOffFurni(RoomUser user, Item item)
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
                    roomItem.OnUserWalksOffFurni -= this.delegateFunction;
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

            this.Delay = row[0] == null ? 0 : Convert.ToInt32(row[0].ToString());

            string itemslist = row["triggers_item"].ToString();

            if (itemslist == "")
                return;

            foreach (string item in itemslist.Split(';'))
            {
                Item roomItem = insideRoom.GetRoomItemHandler().GetItem(Convert.ToInt32(item));
                if (roomItem != null && !this.items.Contains(roomItem) && roomItem.Id != this.item.Id)
                {
                    roomItem.OnUserWalksOffFurni += this.delegateFunction;
                    this.items.Add(roomItem);
                }
            }
        }

        public void OnTrigger(GameClient Session, int SpriteId)
        {
            ServerPacket Message10 = new ServerPacket(ServerPacketHeader.WiredEffectConfigMessageComposer);
            Message10.WriteBoolean(false);
            Message10.WriteInteger(10);
            Message10.WriteInteger(this.items.Count);
            foreach (Item roomItem in this.items)
                Message10.WriteInteger(roomItem.Id);
            Message10.WriteInteger(SpriteId);
            Message10.WriteInteger(this.item.Id);
            Message10.WriteString("");
            Message10.WriteInteger(0);
            Message10.WriteInteger(8);
            Message10.WriteInteger(0);
            Message10.WriteInteger(this.Delay);
            Message10.WriteInteger(0);
            Message10.WriteInteger(0);
            Session.SendPacket(Message10);
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
