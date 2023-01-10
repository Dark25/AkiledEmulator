using Akiled.Communication.Packets.Outgoing;
using Akiled.Database.Interfaces;
using Akiled.HabboHotel.GameClients;
using Akiled.HabboHotel.Items;
using Akiled.HabboHotel.Rooms.Wired.WiredHandlers.Interfaces;
using System;
using System.Data;

namespace Akiled.HabboHotel.Rooms.Wired.WiredHandlers.Effects
{
    public class TimerReset : IWiredEffect, IWired, IWiredCycleable
    {
        private Room room;
        private readonly int itemID;
        private WiredHandler handler;
        public int Delay { get; set; }
        private bool disposed;

        public TimerReset(Room room, WiredHandler handler, int delay, int itemID)
        {
            this.room = room;
            this.handler = handler;
            this.Delay = delay;
            this.disposed = false;
            this.itemID = itemID;
        }

        public void Handle(RoomUser user, Item TriggerItem)
        {
            if (this.Delay > 0)
                this.handler.RequestCycle(new WiredCycle(this, user, TriggerItem, this.Delay));
            else
                this.ResetTimers();
        }

        public bool OnCycle(RoomUser user, Item item)
        {
            this.ResetTimers();
            return false;
        }

        public void Dispose()
        {
            this.disposed = true;
            this.room = (Room)null;
            this.handler = (WiredHandler)null;
        }

        private void ResetTimers()
        {
            this.handler.TriggerTimer();
            this.room.lastTimerReset = DateTime.Now;
        }

        public void SaveToDatabase(IQueryAdapter dbClient)
        {
            WiredUtillity.SaveTriggerItem(dbClient, this.itemID, string.Empty, this.Delay.ToString(), false, null);
        }

        public void LoadFromDatabase(IQueryAdapter dbClient, Room insideRoom)
        {
            dbClient.SetQuery("SELECT trigger_data, triggers_item FROM wired_items WHERE trigger_id = @id ");
            dbClient.AddParameter("id", this.itemID);
            DataRow row = dbClient.GetRow();

            this.Delay = 0;

            if (row == null)
                return;

            this.Delay = row == null ? 0 : Convert.ToInt32(row[0].ToString());
        }

        public void OnTrigger(GameClient Session, int SpriteId)
        {
            ServerPacket Message14 = new ServerPacket(ServerPacketHeader.WiredEffectConfigMessageComposer);
            Message14.WriteBoolean(false);
            Message14.WriteInteger(5);
            Message14.WriteInteger(0);
            Message14.WriteInteger(SpriteId);
            Message14.WriteInteger(this.itemID);
            Message14.WriteString(this.Delay.ToString());
            Message14.WriteInteger(0);
            Message14.WriteInteger(0);
            Message14.WriteInteger(1);
            Message14.WriteInteger(this.Delay);
            Message14.WriteInteger(0);
            Message14.WriteInteger(0);
            Session.SendPacket(Message14);
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
