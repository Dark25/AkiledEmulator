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
    public class ToggleItemState : IWired, IWiredCycleable, IWiredEffect
    {
        private readonly Item item;
        private Gamemap gamemap;
        private WiredHandler handler;
        private readonly List<Item> items;
        public int Delay { get; set; }
        private bool disposed;

        public ToggleItemState(Gamemap gamemap, WiredHandler handler, List<Item> items, int delay, Item Item)
        {
            this.item = Item;
            this.gamemap = gamemap;
            this.handler = handler;
            this.items = items;
            this.Delay = delay;
            this.disposed = false;
        }

        public bool OnCycle(RoomUser user, Item item)
        {

            this.ToggleItems((RoomUser)user);
            return false;
        }

        public void Handle(RoomUser user, Item TriggerItem)
        {
            if (this.Delay > 0)
                this.handler.RequestCycle(new WiredCycle(this, user, TriggerItem, this.Delay));
            else
                this.ToggleItems(user);
        }

        private bool ToggleItems(RoomUser user)
        {
            bool flag = false;
            foreach (Item roomItem in this.items)
            {
                if (roomItem != null)
                {
                    if (user != null && user.GetClient() != null)
                        roomItem.Interactor.OnTrigger(user.GetClient(), roomItem, 0, true);
                    else
                        roomItem.Interactor.OnTrigger((GameClient)null, roomItem, 0, true);
                    flag = true;
                }
            }
            return flag;
        }

        public void Dispose()
        {
            this.disposed = true;
            this.gamemap = (Gamemap)null;
            this.handler = (WiredHandler)null;
            if (this.items != null)
                this.items.Clear();
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

            this.Delay = row["trigger_data"] == null ? 0 : Convert.ToInt32(row["trigger_data"].ToString());


            string itemlist = row["triggers_item"] == null ? "" : row["triggers_item"].ToString();

            if (itemlist == "")
                return;

            foreach (string item in itemlist.Split(';'))
            {
                Item roomItem = insideRoom.GetRoomItemHandler().GetItem(Convert.ToInt32(item));
                if (roomItem != null && !this.items.Contains(roomItem) && roomItem.Id != this.item.Id)
                    this.items.Add(roomItem);
            }
        }

        public void OnTrigger(GameClient Session, int SpriteId)
        {
            ServerPacket Message17 = new ServerPacket(ServerPacketHeader.WiredEffectConfigMessageComposer);
            Message17.WriteBoolean(false);
            Message17.WriteInteger(10);
            Message17.WriteInteger(this.items.Count);
            foreach (Item roomItem in this.items)
                Message17.WriteInteger(roomItem.Id);
            Message17.WriteInteger(SpriteId);
            Message17.WriteInteger(this.item.Id);
            Message17.WriteString(this.Delay.ToString());
            Message17.WriteInteger(0);
            Message17.WriteInteger(8);
            Message17.WriteInteger(0);
            Message17.WriteInteger(this.Delay); //Seconde
            Message17.WriteInteger(0);
            Message17.WriteInteger(0);
            Session.SendPacket(Message17);
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
