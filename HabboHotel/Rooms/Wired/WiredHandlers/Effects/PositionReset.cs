using Akiled.Communication.Packets.Outgoing;
using Akiled.Database.Interfaces;
using Akiled.HabboHotel.GameClients;
using Akiled.HabboHotel.Items;
using Akiled.HabboHotel.Rooms.Wired.WiredHandlers.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;

namespace Akiled.HabboHotel.Rooms.Wired.WiredHandlers.Effects
{
    public class PositionReset : IWired, IWiredCycleable, IWiredEffect
    {
        private RoomItemHandling roomItemHandler;
        private WiredHandler handler;
        private readonly int itemID;
        private ConcurrentDictionary<int, ItemsPosReset> items;
        public int Delay { get; set; }
        private bool disposed;

        private int EtatActuel;
        private int DirectionActuel;
        private int PositionActuel;

        public PositionReset(List<Item> items, int delay, RoomItemHandling roomItemHandler, WiredHandler handler, int itemID, int etatActuel, int directionActuel, int positionActuel)
        {
            this.Delay = delay;
            this.roomItemHandler = roomItemHandler;
            this.itemID = itemID;
            this.handler = handler;
            this.disposed = false;

            this.EtatActuel = etatActuel;
            this.DirectionActuel = directionActuel;
            this.PositionActuel = positionActuel;

            this.items = new ConcurrentDictionary<int, ItemsPosReset>();

            foreach (Item roomItem in items)
            {
                if (!this.items.ContainsKey(roomItem.Id))
                    this.items.TryAdd(roomItem.Id, new ItemsPosReset(roomItem, roomItem.GetX, roomItem.GetY, roomItem.GetZ, roomItem.Rotation, roomItem.ExtraData));
                else
                {
                    ItemsPosReset RemoveItem = null;
                    this.items.TryRemove(roomItem.Id, out RemoveItem);
                    this.items.TryAdd(roomItem.Id, new ItemsPosReset(roomItem, roomItem.GetX, roomItem.GetY, roomItem.GetZ, roomItem.Rotation, roomItem.ExtraData));
                }
            }
        }

        public bool OnCycle(RoomUser user, Item item)
        {
            this.HandleItems();
            return false;
        }

        public void Handle(RoomUser user, Item TriggerItem)
        {
            if (this.Delay > 0)
                this.handler.RequestCycle(new WiredCycle(this, user, TriggerItem, this.Delay));
            else
                this.HandleItems();
        }

        private void HandleItems()
        {
            foreach (ItemsPosReset roomItem in this.items.Values)
            {
                this.HandleReset(roomItem);
            }
        }

        private void HandleReset(ItemsPosReset roomItem)
        {
            if (roomItem == null)
                return;
            if (this.EtatActuel == 1)
            {
                if (roomItem.extradata != "Null")
                {
                    if (roomItem.item.ExtraData != roomItem.extradata)
                    {
                        roomItem.item.ExtraData = roomItem.extradata;
                        roomItem.item.UpdateState();
                        roomItem.item.GetRoom().GetGameMap().updateMapForItem(roomItem.item);
                    }
                }
            }

            if (this.DirectionActuel == 1)
            {
                if (roomItem.rot != roomItem.item.Rotation)
                    this.roomItemHandler.RotReset(roomItem.item, roomItem.rot);
            }

            if (this.PositionActuel == 1)
            {
                if (roomItem.x != roomItem.item.GetX || roomItem.y != roomItem.item.GetY || roomItem.z != roomItem.item.GetZ)
                    this.roomItemHandler.PositionReset(roomItem.item, roomItem.x, roomItem.y, roomItem.z);
            }
        }

        public void Dispose()
        {
            this.disposed = true;
            this.roomItemHandler = (RoomItemHandling)null;
            this.handler = (WiredHandler)null;
            if (this.items != null)
                this.items.Clear();
            this.items = null;
        }

        public void SaveToDatabase(IQueryAdapter dbClient)
        {
            //WiredUtillity.SaveTriggerItem(dbClient, this.itemID, "integer", rotationandmove, this.delay.ToString(), false, this.items);
            string triggersitem = "";

            int i = 0;
            foreach (ItemsPosReset roomItem in this.items.Values)
            {
                if (i != 0)
                    triggersitem += ";";

                triggersitem += roomItem.item.Id + ":" + roomItem.x + ":" + roomItem.y + ":" + roomItem.z + ":" + roomItem.rot + ":" + roomItem.extradata;

                i++;
            }

            string triggerData2 = this.EtatActuel + ";" + this.DirectionActuel + ";" + this.PositionActuel;

            dbClient.RunQuery("DELETE FROM wired_items WHERE trigger_id = " + this.itemID);
            dbClient.SetQuery("INSERT INTO wired_items (trigger_id,trigger_data,trigger_data_2,all_user_triggerable,triggers_item) VALUES (@id,@trigger_data,@trigger_data_2,@triggerable,@triggers_item)");
            dbClient.AddParameter("id", this.itemID);
            dbClient.AddParameter("trigger_data", this.Delay.ToString());
            dbClient.AddParameter("trigger_data_2", triggerData2);
            dbClient.AddParameter("triggerable", 0);
            dbClient.AddParameter("triggers_item", triggersitem);
            dbClient.RunQuery();
        }

        public void LoadFromDatabase(IQueryAdapter dbClient, Room insideRoom)
        {
            dbClient.SetQuery("SELECT trigger_data, trigger_data_2, triggers_item FROM wired_items WHERE trigger_id = @id ");
            dbClient.AddParameter("id", this.itemID);
            DataRow row = dbClient.GetRow();

            this.Delay = 20;

            if (row == null)
                return;

            int result;
            this.Delay = (int.TryParse(row["trigger_data"].ToString(), out result)) ? result : 20;

            string itemslist = row["triggers_item"].ToString();

            string data2 = row["trigger_data_2"].ToString();

            if (data2.Length == 5)
            {
                this.EtatActuel = Convert.ToInt32(data2.Split(';')[0]);
                this.DirectionActuel = Convert.ToInt32(data2.Split(';')[1]);
                this.PositionActuel = Convert.ToInt32(data2.Split(';')[2]);
            }

            if (itemslist == "")
                return;

            foreach (string item in itemslist.Split(';'))
            {
                string[] Item2 = item.Split(':');
                if (Item2.Length != 6)
                    continue;
                Item roomItem = insideRoom.GetRoomItemHandler().GetItem(Convert.ToInt32(Item2[0]));
                if (roomItem != null && !this.items.ContainsKey(roomItem.Id) && roomItem.Id != this.itemID)
                    this.items.TryAdd(roomItem.Id, new ItemsPosReset(roomItem, Convert.ToInt32(Item2[1]), Convert.ToInt32(Item2[2]), Convert.ToDouble(Item2[3]), Convert.ToInt32(Item2[4]), Item2[5]));
            }
        }

        public void OnTrigger(GameClient Session, int SpriteId)
        {
            ServerPacket Message12 = new ServerPacket(ServerPacketHeader.WiredEffectConfigMessageComposer);
            Message12.WriteBoolean(false);
            Message12.WriteInteger(10);
            Message12.WriteInteger(this.items.Count);
            foreach (int roomItemId in this.items.Keys)
                Message12.WriteInteger(roomItemId);
            Message12.WriteInteger(SpriteId);
            Message12.WriteInteger(this.itemID);
            Message12.WriteString("");
            Message12.WriteInteger(3);

            Message12.WriteInteger(this.EtatActuel); //Etat actuel du mobi
            Message12.WriteInteger(this.DirectionActuel); //Direction  actuelle
            Message12.WriteInteger(this.PositionActuel); //position actuelle dans l'appart

            Message12.WriteInteger(1);
            Message12.WriteInteger(3);
            Message12.WriteInteger(this.Delay);
            Message12.WriteInteger(0);
            Session.SendPacket(Message12);
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

    public class ItemsPosReset
    {
        public Item item;

        public int x;
        public int y;
        public double z;
        public int rot;
        public string extradata;

        public ItemsPosReset(Item Item, int X, int Y, double Z, int Rot, string Extradata)
        {
            this.item = Item;

            this.x = X;
            this.y = Y;
            this.z = Z;
            this.rot = Rot;

            int result;
            if (int.TryParse(Extradata, out result) || (!Extradata.Contains(";") && !Extradata.Contains(":")))
            {
                if (Item.GetBaseItem().InteractionType != InteractionType.dice)
                    this.extradata = Extradata;
                else
                    this.extradata = "Null";
            }
            else
                this.extradata = "Null";
        }

    }
}
