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
    public class MoveRotate : IWiredEffect, IWired, IWiredCycleable
    {
        private Room room;
        private WiredHandler handler;
        private readonly int itemID;
        private MovementState movement;
        private RotationState rotation;
        private List<Item> items;
        public int Delay { get; set; }
        private bool isDisposed;

        public MoveRotate(MovementState movement, RotationState rotation, List<Item> items, int delay, Room room, WiredHandler handler, int itemID)
        {
            this.movement = movement;
            this.rotation = rotation;
            this.items = items;
            this.Delay = delay;
            this.room = room;
            this.handler = handler;
            this.itemID = itemID;
            this.isDisposed = false;
        }

        public void Handle(RoomUser user, Item TriggerItem)
        {
            if (this.Delay > 0)
                this.handler.RequestCycle(new WiredCycle(this, user, TriggerItem, this.Delay));
            else
                this.HandleItems();
        }

        public bool OnCycle(RoomUser user, Item item)
        {
            this.HandleItems();
            return false;
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

        private bool HandleMovement(Item item)
        {
            if (this.room.GetRoomItemHandler().GetItem(item.Id) == null)
                return false;

            Point newPoint = MovementManagement.HandleMovement(item.Coordinate, this.movement);
            int newRot = MovementManagement.HandleRotation(item.Rotation, this.rotation);


            if (newPoint != item.Coordinate || newRot != item.Rotation)
            {
                int OldX = item.GetX;
                int OldY = item.GetY;
                double OldZ = item.GetZ;
                if (this.room.GetRoomItemHandler().SetFloorItem(null, item, newPoint.X, newPoint.Y, newRot, false, false, (newRot != item.Rotation)))
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
                    this.room.SendPacket(Message);
                }
            }
            return false;
        }

        public void SaveToDatabase(IQueryAdapter dbClient)
        {
            string rotationandmove = (int)this.rotation + ";" + (int)this.movement;
            WiredUtillity.SaveTriggerItem(dbClient, this.itemID, rotationandmove, this.Delay.ToString(), false, this.items);
        }

        public void LoadFromDatabase(IQueryAdapter dbClient, Room insideRoom)
        {
            try
            {
                string data2 = null;
                string wireditem = null;

                dbClient.SetQuery("SELECT trigger_data, trigger_data_2, triggers_item FROM wired_items WHERE trigger_id = @id ");
                dbClient.AddParameter("id", this.itemID);
                DataRow row = dbClient.GetRow();
                if (row != null)
                {
                    this.Delay = Convert.ToInt32(row["trigger_data"]);

                    data2 = row["trigger_data_2"].ToString();

                    wireditem = row["triggers_item"].ToString();
                }
                if (data2 != null)
                {
                    int rotationint = 0;
                    int.TryParse(data2.Split(';')[0], out rotationint);

                    int movementint = 0;
                    int.TryParse(data2.Split(';')[1], out movementint);

                    this.rotation = (RotationState)rotationint;
                    this.movement = (MovementState)movementint;
                }
                else
                {
                    this.rotation = RotationState.none;
                    this.movement = MovementState.none;
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
            ServerPacket Message13 = new ServerPacket(ServerPacketHeader.WiredEffectConfigMessageComposer);
            Message13.WriteBoolean(false);
            Message13.WriteInteger(10);
            Message13.WriteInteger(this.items.Count);
            foreach (Item roomItem in this.items)
                Message13.WriteInteger(roomItem.Id);
            Message13.WriteInteger(SpriteId);
            Message13.WriteInteger(this.itemID);
            Message13.WriteString("");
            Message13.WriteInteger(2);
            Message13.WriteInteger((int)this.movement);
            Message13.WriteInteger((int)this.rotation);
            Message13.WriteInteger(0);
            Message13.WriteInteger(4);
            Message13.WriteInteger(this.Delay);
            Message13.WriteInteger(0);
            Message13.WriteInteger(0);
            Session.SendPacket(Message13);
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
