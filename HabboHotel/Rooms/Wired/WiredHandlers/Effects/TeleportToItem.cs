using Akiled.Communication.Packets.Outgoing;
using Akiled.Database.Interfaces;
using Akiled.HabboHotel.GameClients;
using Akiled.HabboHotel.Items;
using Akiled.HabboHotel.Rooms.Wired.WiredHandlers.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Akiled.HabboHotel.Rooms.Wired.WiredHandlers.Effects
{
    public class TeleportToItem : IWired, IWiredCycleable, IWiredEffect
    {
        private Gamemap gamemap;
        private WiredHandler handler;
        private List<Item> items;
        public int Delay { get; set; }

        private readonly int itemID;
        private bool disposed;

        public TeleportToItem(Gamemap gamemap, WiredHandler handler, List<Item> items, int delay, int itemID)
        {
            this.gamemap = gamemap;
            this.handler = handler;
            this.items = items;
            this.Delay = delay;
            this.itemID = itemID;
            this.disposed = false;
        }

        public bool OnCycle(RoomUser user, Item item)
        {
            if (user != null)
                TeleportUser(user);
            return false;
        }

        private void DoAnimation(RoomUser user)
        {
            user.ApplyEffect(4, true);
            user.Freeze = true;
        }
        private void ResetAnimation(RoomUser user)
        {
            user.ApplyEffect(user.CurrentEffect, true);
            if (user.FreezeEndCounter <= 0)
                user.Freeze = false;
        }

        public void Handle(RoomUser user, Item TriggerItem)
        {
            if (this.items.Count == 0)
                return;
            if (user == null)
                return;
            DoAnimation(user);
            this.handler.RequestCycle(new WiredCycle(this, user, null, this.Delay));
        }

        private void TeleportUser(RoomUser user)
        {
            if (user == null)
                return;

            if (this.items.Count > 1)
            {
                Item roomItem = this.items[AkiledEnvironment.GetRandomNumber(0, this.items.Count - 1)];
                if (roomItem == null)
                    return;
                if (roomItem.Coordinate != user.Coordinate)
                {
                    this.gamemap.TeleportToItem(user, roomItem);
                }
            }
            else if (this.items.Count == 1)
            {
                this.gamemap.TeleportToItem(user, Enumerable.First<Item>((IEnumerable<Item>)this.items));
            }
            ResetAnimation(user);
            return;
        }

        public void Dispose()
        {
            this.disposed = true;
            this.gamemap = (Gamemap)null;
            this.handler = (WiredHandler)null;
            if (this.items != null)
                this.items.Clear();
            this.items = (List<Item>)null;
        }

        public void SaveToDatabase(IQueryAdapter dbClient)
        {
            WiredUtillity.SaveTriggerItem(dbClient, this.itemID, string.Empty, this.Delay.ToString(), false, this.items);
        }

        public void LoadFromDatabase(IQueryAdapter dbClient, Room insideRoom)
        {
            dbClient.SetQuery("SELECT trigger_data, triggers_item FROM wired_items WHERE trigger_id = @id ");
            dbClient.AddParameter("id", this.itemID);
            DataRow row = dbClient.GetRow();
            this.Delay = 0;
            if (row == null)
                return;

            this.Delay = row["trigger_data"] == null ? 0 : Convert.ToInt32(row["trigger_data"].ToString());

            string itemslist = row["triggers_item"].ToString();

            if (itemslist == "")
                return;
            foreach (string item in itemslist.Split(';'))
            {
                Item roomItem = insideRoom.GetRoomItemHandler().GetItem(Convert.ToInt32(item));
                if (roomItem != null && !this.items.Contains(roomItem) && roomItem.Id != this.itemID)
                {
                    this.items.Add(roomItem);
                }
            }
        }

        public void OnTrigger(GameClient Session, int SpriteId)
        {
            ServerPacket Message16 = new ServerPacket(ServerPacketHeader.WiredEffectConfigMessageComposer);
            Message16.WriteBoolean(false);
            Message16.WriteInteger(10);
            Message16.WriteInteger(this.items.Count);
            foreach (Item roomItem in this.items)
                Message16.WriteInteger(roomItem.Id);
            Message16.WriteInteger(SpriteId);
            Message16.WriteInteger(this.itemID);
            Message16.WriteString("");
            Message16.WriteInteger(0);
            Message16.WriteInteger(8);
            Message16.WriteInteger(0);
            Message16.WriteInteger(this.Delay);
            Message16.WriteInteger(0);
            Message16.WriteInteger(0);
            Session.SendPacket(Message16);
        }

        public void DeleteFromDatabase(IQueryAdapter dbClient)
        {
            dbClient.RunQuery("DELETE FROM wired_items WHERE trigger_id = '" + this.itemID + "'");
        }

        public bool Disposed()
        {
            return this.disposed;
        }
    }
}
