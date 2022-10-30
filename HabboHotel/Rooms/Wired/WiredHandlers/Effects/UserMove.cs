using Akiled.Communication.Packets.Outgoing;
using Akiled.Database.Interfaces;
using Akiled.HabboHotel.GameClients;
using Akiled.HabboHotel.Items;
using Akiled.HabboHotel.Rooms.Wired.WiredHandlers.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;

namespace Akiled.HabboHotel.Rooms.Wired.WiredHandlers.Effects
{
    public class UserMove : IWired, IWiredEffect, IWiredCycleable
    {
        private readonly WiredHandler handler;
        private readonly int itemID;
        private List<Item> items;
        public int Delay { get; set; }
        private bool disposed;

        public UserMove(List<Item> items, int pDelay, WiredHandler handler, int itemID)
        {
            this.itemID = itemID;
            this.handler = handler;
            this.items = items;
            this.Delay = pDelay;
            this.disposed = false;
        }

        private void Execute(RoomUser User)
        {
            if (this.items.Count == 0)
                return;

            Item roomItem = this.items[0];
            if (roomItem == null)
                return;
            if (roomItem.Coordinate != User.Coordinate)
            {
                User.IsWalking = true;
                User.GoalX = roomItem.GetX;
                User.GoalY = roomItem.GetY;
            }
        }

        public bool OnCycle(RoomUser User, Item Item)
        {
            this.Execute(User);
            return false;
        }

        public void Handle(RoomUser User, Item TriggerItem)
        {
            if (this.Delay > 0)
                this.handler.RequestCycle(new WiredCycle(this, User, TriggerItem, this.Delay));
            else
                this.Execute(User);
        }

        public void Dispose()
        {
            this.disposed = true;
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
            dbClient.SetQuery("SELECT triggers_item, trigger_data FROM wired_items WHERE trigger_id = @id ");
            dbClient.AddParameter("id", this.itemID);
            DataRow row = dbClient.GetRow();
            if (row == null)
                return;

            int result;
            this.Delay = (int.TryParse(row["trigger_data"].ToString(), out result)) ? result : 0;

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
            ServerPacket Message15 = new ServerPacket(ServerPacketHeader.WiredEffectConfigMessageComposer);
            Message15.WriteBoolean(false);
            Message15.WriteInteger(1);
            Message15.WriteInteger(this.items.Count);
            foreach (Item roomItem in this.items)
                Message15.WriteInteger(roomItem.Id);
            Message15.WriteInteger(SpriteId);
            Message15.WriteInteger(this.itemID);
            Message15.WriteString("");
            Message15.WriteInteger(0);
            Message15.WriteInteger(0);
            Message15.WriteInteger(12);
            Message15.WriteInteger(this.Delay);
            Message15.WriteInteger(0);
            Message15.WriteInteger(0);
            Session.SendPacket(Message15);
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
