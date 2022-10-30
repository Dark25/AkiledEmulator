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
    public class ExecutePile : IWired, IWiredEffect, IWiredCycleable
    {
        private WiredHandler handler;
        private readonly Item item;
        private List<Item> items;
        public int Delay { get; set; }
        private bool disposed;

        public ExecutePile(List<Item> items, int mDeley, WiredHandler handler, Item item)
        {
            this.Delay = mDeley;
            this.disposed = false;

            this.items = items;
            this.item = item;
            this.handler = handler;
        }

        public bool Disposed()
        {
            return this.disposed;
        }

        public bool OnCycle(RoomUser user, Item item)
        {
            this.HandleEffect(user, item);
            return false;
        }

        private void HandleEffect(RoomUser user, Item TriggerItem)
        {
            foreach (Item roomItem in this.items)
            {
                if (roomItem.Coordinate != this.item.Coordinate)
                    this.handler.ExecutePile(roomItem.Coordinate, user, TriggerItem);
            }
        }

        public void Handle(RoomUser user, Item TriggerItem)
        {
            if (this.Delay > 0)
                this.handler.RequestCycle(new WiredCycle(this, user, TriggerItem, this.Delay));
            else
                this.HandleEffect(user, TriggerItem);
        }

        public void Dispose()
        {
            this.disposed = true;
            this.handler = (WiredHandler)null;
            if (this.items != null)
                this.items.Clear();
            this.items = (List<Item>)null;
        }

        public void SaveToDatabase(IQueryAdapter dbClient)
        {
            WiredUtillity.SaveTriggerItem(dbClient, this.item.Id, this.Delay.ToString(), string.Empty, false, this.items);
        }

        public void LoadFromDatabase(IQueryAdapter dbClient, Room insideRoom)
        {
            try
            {
                string wireditem = null;

                dbClient.SetQuery("SELECT trigger_data_2, triggers_item FROM wired_items WHERE trigger_id = @id ");
                dbClient.AddParameter("id", this.item.Id);
                DataRow row = dbClient.GetRow();
                if (row == null)
                    return;

                wireditem = row["triggers_item"].ToString();

                int result;
                this.Delay = (int.TryParse(row["trigger_data_2"].ToString(), out result)) ? result : 0;

                if (wireditem == "" || wireditem == null)
                    return;

                foreach (string itemid in wireditem.Split(';'))
                {
                    Item roomItem = insideRoom.GetRoomItemHandler().GetItem(Convert.ToInt32(itemid));
                    if (roomItem != null && !this.items.Contains(roomItem) && roomItem.Id != this.item.Id)
                        this.items.Add(roomItem);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Wired id : " + this.item.Id + " erreur :" + ex);
            }
        }

        public void OnTrigger(GameClient Session, int SpriteId)
        {
            ServerPacket Message = new ServerPacket(ServerPacketHeader.WiredEffectConfigMessageComposer);
            Message.WriteBoolean(false);
            Message.WriteInteger(10);
            Message.WriteInteger(this.items.Count);
            foreach (Item roomItem in this.items)
                Message.WriteInteger(roomItem.Id);
            Message.WriteInteger(SpriteId);
            Message.WriteInteger(this.item.Id);
            Message.WriteString("");
            Message.WriteInteger(0);

            Message.WriteInteger(0);
            Message.WriteInteger(0);
            Message.WriteInteger(this.Delay);

            Message.WriteInteger(0);
            Session.SendPacket(Message);
        }

        public void DeleteFromDatabase(IQueryAdapter dbClient)
        {
            dbClient.RunQuery("DELETE FROM wired_items WHERE trigger_id = '" + this.item.Id + "'");
        }
    }
}
