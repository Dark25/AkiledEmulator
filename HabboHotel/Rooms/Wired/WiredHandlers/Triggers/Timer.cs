using Akiled.Communication.Packets.Outgoing;
using Akiled.Database.Interfaces;
using Akiled.HabboHotel.GameClients;
using Akiled.HabboHotel.Items;
using Akiled.HabboHotel.Rooms.Games;
using Akiled.HabboHotel.Rooms.Wired.WiredHandlers.Interfaces;
using System;
using System.Data;

namespace Akiled.HabboHotel.Rooms.Wired.WiredHandlers.Triggers
{
    public class Timer : IWired, IWiredCycleable
    {
        private Item item;
        private WiredHandler handler;
        public int Delay { get; set; }
        private readonly RoomEventDelegate delegateFunction;
        private bool disposed;

        public Timer(Item item, WiredHandler handler, int cycleCount, GameManager gameManager)
        {
            this.item = item;
            this.handler = handler;
            this.Delay = cycleCount;
            this.delegateFunction = new RoomEventDelegate(this.ResetTimer);
            this.handler.TrgTimer += this.delegateFunction;
            this.disposed = false;
        }

        public void ResetTimer(object sender, EventArgs e)
        {
            this.handler.RequestCycle(new WiredCycle(this, null, null, this.Delay));
        }

        public bool OnCycle(RoomUser user, Item item)
        {
            this.handler.ExecutePile(this.item.Coordinate, (RoomUser)null, null);
            return false;
        }

        public void Dispose()
        {
            this.disposed = true;
            this.item = (Item)null;
            this.handler.TrgTimer -= this.delegateFunction;
            this.handler = (WiredHandler)null;
        }

        public void SaveToDatabase(IQueryAdapter dbClient)
        {
            WiredUtillity.SaveTriggerItem(dbClient, this.item.Id, string.Empty, this.Delay.ToString(), false, null);
        }

        public void LoadFromDatabase(IQueryAdapter dbClient, Room insideRoom)
        {
            dbClient.SetQuery("SELECT trigger_data FROM wired_items WHERE trigger_id = @id ");
            dbClient.AddParameter("id", this.item.Id);
            DataRow row = dbClient.GetRow();
            if (row != null)
                this.Delay = Convert.ToInt32(row[0].ToString());
            else
                this.Delay = 20;
            if (this.Delay == 0)
                this.Delay = 2;
        }

        public void DeleteFromDatabase(IQueryAdapter dbClient)
        {
            dbClient.RunQuery("DELETE FROM wired_items WHERE trigger_id = '" + this.item.Id + "'");
        }

        public void OnTrigger(GameClient Session, int SpriteId)
        {
            ServerPacket Message1 = new ServerPacket(ServerPacketHeader.WiredTriggerConfigMessageComposer);
            Message1.WriteBoolean(false);
            Message1.WriteInteger(5);
            Message1.WriteInteger(0);
            Message1.WriteInteger(SpriteId);
            Message1.WriteInteger(this.item.Id);
            Message1.WriteString("");
            Message1.WriteInteger(1);
            Message1.WriteInteger(this.Delay);
            Message1.WriteInteger(1);
            Message1.WriteInteger(3);
            Message1.WriteInteger(0);
            Message1.WriteInteger(0);
            Session.SendPacket(Message1);
        }

        public bool Disposed()
        {
            return this.disposed;
        }
    }
}
