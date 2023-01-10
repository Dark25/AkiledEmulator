using Akiled.Communication.Packets.Outgoing;
using Akiled.Database.Interfaces;
using Akiled.HabboHotel.GameClients;
using Akiled.HabboHotel.Items;
using Akiled.HabboHotel.Rooms.Wired.WiredHandlers.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;

namespace Akiled.HabboHotel.Rooms.Wired.WiredHandlers.Conditions
{
    public class FurniStatePosMatchNegative : IWiredCondition, IWired
    {
        private readonly int itemID;
        private Dictionary<int, ItemsPosReset> items;
        private bool isDisposed;

        private int EtatActuel;
        private int DirectionActuel;
        private int PositionActuel;

        public FurniStatePosMatchNegative(Item item, List<Item> items, int etatActuel, int directionActuel, int positionActuel)
        {
            this.itemID = item.Id;
            this.isDisposed = false;

            this.EtatActuel = etatActuel;
            this.DirectionActuel = directionActuel;
            this.PositionActuel = positionActuel;

            this.items = new Dictionary<int, ItemsPosReset>();

            foreach (Item roomItem in items)
            {
                if (!this.items.ContainsKey(roomItem.Id))
                    this.items.Add(roomItem.Id, new ItemsPosReset(roomItem, roomItem.GetX, roomItem.GetY, roomItem.GetZ, roomItem.Rotation, roomItem.ExtraData));
                else
                {
                    this.items.Remove(roomItem.Id);
                    this.items.Add(roomItem.Id, new ItemsPosReset(roomItem, roomItem.GetX, roomItem.GetY, roomItem.GetZ, roomItem.Rotation, roomItem.ExtraData));
                }
            }
        }

        public bool AllowsExecution(RoomUser user, Item TriggerItem)
        {
            foreach (ItemsPosReset roomItem in this.items.Values)
            {
                if (this.EtatActuel == 1)
                {
                    if (roomItem.extradata != "Null")
                    {
                        if (!(roomItem.item.ExtraData == "" && roomItem.extradata == "0") && !(roomItem.item.ExtraData == "0" && roomItem.extradata == ""))
                        {
                            if (roomItem.item.ExtraData == roomItem.extradata)
                                return false;
                        }
                    }
                }

                if (this.DirectionActuel == 1)
                {
                    if (roomItem.rot == roomItem.item.Rotation)
                        return false;
                }

                if (this.PositionActuel == 1)
                {
                    if (roomItem.x == roomItem.item.GetX && roomItem.y == roomItem.item.GetY)
                        return false;
                }
            }
            return true;
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
            dbClient.AddParameter("trigger_data", "");
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

            if (row == null)
                return;

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
                    this.items.Add(roomItem.Id, new ItemsPosReset(roomItem, Convert.ToInt32(Item2[1]), Convert.ToInt32(Item2[2]), Convert.ToDouble(Item2[3]), Convert.ToInt32(Item2[4]), Item2[5]));
            }
        }

        public void OnTrigger(GameClient Session, int SpriteId)
        {
            ServerPacket Message19 = new ServerPacket(ServerPacketHeader.WiredConditionConfigMessageComposer);
            Message19.WriteBoolean(false);
            Message19.WriteInteger(10);
            Message19.WriteInteger(this.items.Count);
            foreach (int roomItemid in this.items.Keys)
                Message19.WriteInteger(roomItemid);
            Message19.WriteInteger(SpriteId);
            Message19.WriteInteger(this.itemID);
            Message19.WriteString("");
            Message19.WriteInteger(3);

            Message19.WriteInteger(this.EtatActuel); //Etat actuel du mobi
            Message19.WriteInteger(this.DirectionActuel); //Direction  actuelle
            Message19.WriteInteger(this.PositionActuel); //position actuelle dans l'appart

            Message19.WriteInteger(0);
            Message19.WriteInteger(0);
            Session.SendPacket(Message19);
        }

        public void DeleteFromDatabase(IQueryAdapter dbClient)
        {
            dbClient.RunQuery("DELETE FROM wired_items WHERE trigger_id = '" + this.itemID + "'");
        }

        public void Dispose()
        {
            this.isDisposed = true;
            if (this.items != null)
                this.items.Clear();
            this.items = null;
        }

        public bool Disposed()
        {
            return this.isDisposed;
        }
    }
}
