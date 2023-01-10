using Akiled.Communication.Packets.Outgoing;
using Akiled.Database.Interfaces;
using Akiled.HabboHotel.GameClients;
using Akiled.HabboHotel.Items;
using Akiled.HabboHotel.Rooms.Map.Movement;
using Akiled.HabboHotel.Rooms.Wired.WiredHandlers.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;

namespace Akiled.HabboHotel.Rooms.Wired.WiredHandlers.Effects
{
    public class Escape : IWiredEffect, IWired
    {
        private Room room;
        private WiredHandler handler;
        private readonly int itemID;
        private List<Item> items;
        private bool isDisposed;

        public Escape(List<Item> items, Room room, WiredHandler handler, int itemID)
        {
            this.items = items;
            this.room = room;
            this.handler = handler;
            this.itemID = itemID;
            this.isDisposed = false;
        }

        public void Handle(RoomUser user, Item TriggerItem)
        {
            this.HandleItems();
        }

        public void Dispose()
        {
            this.isDisposed = true;
            this.room = (Room)null;
            this.handler = (WiredHandler)null;
            if (this.items != null)
                this.items.Clear();
            this.items = (List<Item>)null;
        }

        private void HandleItems()
        {
            foreach (Item roomItem in this.items.ToArray())
            {
                this.HandleMovement(roomItem);
            }
        }

        private void HandleMovement(Item item)
        {
            if (room.GetRoomItemHandler().GetItem(item.Id) == null)
                return;

            RoomUser roomUser = room.GetGameMap().SquareHasUserNear(item.GetX, item.GetY);
            if (roomUser != null)
            {
                this.handler.TriggerCollision(roomUser, item);
                return;
            }

            item.movement = room.GetGameMap().GetEscapeMovement(item.GetX, item.GetY, item.movement);
            if (item.movement == MovementState.none)
                return;

            Point newPoint = MovementManagement.HandleMovement(item.Coordinate, item.movement);

            if (newPoint != item.Coordinate)
            {
                int OldX = item.GetX;
                int OldY = item.GetY;
                double OldZ = item.GetZ;
                if (this.room.GetRoomItemHandler().SetFloorItem(null, item, newPoint.X, newPoint.Y, item.Rotation, false, false, false))
                {
                    ServerPacket Message = new ServerPacket(ServerPacketHeader.SlideObjectBundleMessageComposer);
                    Message.WriteInteger(OldX);
                    Message.WriteInteger(OldY);
                    Message.WriteInteger(newPoint.X);
                    Message.WriteInteger(newPoint.Y);
                    Message.WriteInteger(1);
                    Message.WriteInteger(item.Id);
                    Message.WriteString(OldZ.ToString().Replace(',', '.'));
                    Message.WriteString(item.GetZ.ToString().Replace(',', '.'));
                    Message.WriteInteger(0);

                    //ServerMessage Message = new ServerMessage(Outgoing.UpdateItemOnRoom);
                    //item.Serialize(Message, room.OwnerId);
                    this.room.SendPacket(Message);
                }
            }
            return;
        }

        public void SaveToDatabase(IQueryAdapter dbClient)
        {
            WiredUtillity.SaveTriggerItem(dbClient, this.itemID, string.Empty, string.Empty, false, this.items);
        }

        public void LoadFromDatabase(IQueryAdapter dbClient, Room insideRoom)
        {
            try
            {
                string wireditem = null;

                dbClient.SetQuery("SELECT trigger_data, trigger_data_2, triggers_item FROM wired_items WHERE trigger_id = @id ");
                dbClient.AddParameter("id", this.itemID);
                DataRow row = dbClient.GetRow();
                if (row != null)
                {
                    wireditem = row["triggers_item"].ToString();
                }

                if (wireditem == "" || wireditem == null)
                    return;

                foreach (string itemid in wireditem.Split(';'))
                {
                    Item roomItem = insideRoom.GetRoomItemHandler().GetItem(Convert.ToInt32(itemid));
                    if (roomItem != null && !this.items.Contains(roomItem) && roomItem.Id != this.itemID)
                        this.items.Add(roomItem);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Wired id : " + this.itemID + " erreur :" + ex);
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
            Message.WriteInteger(this.itemID);
            Message.WriteString("");
            Message.WriteInteger(0);
            Message.WriteInteger(0);
            Message.WriteInteger(12);
            Message.WriteInteger(0);
            Message.WriteInteger(0);
            Session.SendPacket(Message);
        }

        public void DeleteFromDatabase(IQueryAdapter dbClient)
        {
            dbClient.RunQuery("DELETE FROM wired_items WHERE trigger_id = '" + this.itemID + "'");
        }

        public bool Disposed()
        {
            return this.isDisposed;
        }
    }
}
