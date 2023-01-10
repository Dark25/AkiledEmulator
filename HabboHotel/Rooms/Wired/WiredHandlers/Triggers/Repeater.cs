using Akiled.Communication.Packets.Outgoing;
using Akiled.Database.Interfaces;
using Akiled.HabboHotel.GameClients;
using Akiled.HabboHotel.Items;
using Akiled.HabboHotel.Rooms.Wired.WiredHandlers.Interfaces;

namespace Akiled.HabboHotel.Rooms.Wired.WiredHandlers
{
    public class Repeater : IWired, IWiredCycleable
    {
        public int Delay { get; set; }
        private WiredHandler handler;
        private Item item;
        private bool disposed;

        public Repeater(WiredHandler handler, Item item, int cyclesRequired)
        {
            this.handler = handler;
            this.Delay = cyclesRequired;
            this.item = item;
            this.handler.RequestCycle(new WiredCycle(this, null, null, this.Delay));
            this.disposed = false;
        }

        public bool OnCycle(RoomUser user, Item item)
        {
            this.handler.ExecutePile(this.item.Coordinate, (RoomUser)null, null);
            return true;
        }

        public void Dispose()
        {
            this.disposed = true;
            this.handler = (WiredHandler)null;
            this.item = (Item)null;
        }

        public void SaveToDatabase(IQueryAdapter dbClient)
        {
            WiredUtillity.SaveTriggerItem(dbClient, this.item.Id, string.Empty, this.Delay.ToString(), false, null);
        }

        public void LoadFromDatabase(IQueryAdapter dbClient, Room insideRoom)
        {
            dbClient.SetQuery("SELECT trigger_data FROM wired_items WHERE trigger_id = @id ");
            dbClient.AddParameter("id", this.item.Id);
            this.Delay = dbClient.GetInteger();
        }

        public void DeleteFromDatabase(IQueryAdapter dbClient)
        {
            dbClient.RunQuery("DELETE FROM wired_items WHERE trigger_id = '" + this.item.Id + "'");
        }

        public bool Disposed()
        {
            return this.disposed;
        }
        public void OnTrigger(GameClient Session, int SpriteId)
        {
            ServerPacket Message5 = new ServerPacket(ServerPacketHeader.WiredTriggerConfigMessageComposer);
            Message5.WriteBoolean(false);
            Message5.WriteInteger(5);
            Message5.WriteInteger(0);
            Message5.WriteInteger(SpriteId);
            Message5.WriteInteger(this.item.Id);
            Message5.WriteString("");
            Message5.WriteInteger(1);
            Message5.WriteInteger(this.Delay);
            Message5.WriteInteger(0);
            Message5.WriteInteger(6); //6
            Message5.WriteInteger(0);
            Message5.WriteInteger(0);
            Session.SendPacket(Message5);
        }
    }
}
