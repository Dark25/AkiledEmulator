using Akiled.Communication.Packets.Outgoing;
using Akiled.Database.Interfaces;
using Akiled.HabboHotel.GameClients;
using Akiled.HabboHotel.Items;
using Akiled.HabboHotel.Rooms.Pathfinding;
using Akiled.HabboHotel.Rooms.Wired.WiredHandlers.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;

namespace Akiled.HabboHotel.Rooms.Wired.WiredHandlers.Conditions
{
    public class TriggerUserIsOnFurni : IWiredCondition, IWired
    {
        private Item item;
        private List<Item> items;
        private bool isDisposed;

        public TriggerUserIsOnFurni(Item item, List<Item> items)
        {
            this.item = item;
            this.items = items;
            this.isDisposed = false;
        }

        public bool AllowsExecution(RoomUser user, Item TriggerItem)
        {
            if (user == null)
                return false;

            Point coord;

            foreach (Item roomItem in this.items)
            {
                foreach (ThreeDCoord coor in roomItem.GetAffectedTiles.Values)
                {
                    coord = new Point(coor.X, coor.Y);
                    if (coord == user.Coordinate)
                        return true;
                }
            }
            return false;
        }

        public void SaveToDatabase(IQueryAdapter dbClient)
        {
            WiredUtillity.SaveTriggerItem(dbClient, this.item.Id, string.Empty, string.Empty, false, this.items);
        }

        public void LoadFromDatabase(IQueryAdapter dbClient, Room insideRoom)
        {
            dbClient.SetQuery("SELECT triggers_item FROM wired_items WHERE trigger_id = " + this.item.Id);
            DataRow row = dbClient.GetRow();

            if (row == null)
                return;

            string itemlist = row["triggers_item"].ToString();

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
            ServerPacket Message21 = new ServerPacket(ServerPacketHeader.WiredConditionConfigMessageComposer);
            Message21.WriteBoolean(false);
            Message21.WriteInteger(10);
            Message21.WriteInteger(this.items.Count);
            foreach (Item roomItem in this.items)
                Message21.WriteInteger(roomItem.Id);
            Message21.WriteInteger(SpriteId);
            Message21.WriteInteger(this.item.Id);
            Message21.WriteInteger(0);
            Message21.WriteInteger(0);
            Message21.WriteInteger(0);
            Message21.WriteBoolean(false);
            Message21.WriteBoolean(true);
            Session.SendPacket(Message21);
        }

        public void DeleteFromDatabase(IQueryAdapter dbClient)
        {
            dbClient.RunQuery("DELETE FROM wired_items WHERE trigger_id = '" + this.item.Id + "'");
        }

        public void Dispose()
        {
            this.isDisposed = true;
            this.item = (Item)null;
            if (this.items != null)
                this.items.Clear();
            this.items = (List<Item>)null;
        }

        public bool Disposed()
        {
            return this.isDisposed;
        }
    }
}
