using Akiled.Communication.Packets.Outgoing;
using Akiled.Database.Interfaces;
using Akiled.HabboHotel.GameClients;
using Akiled.HabboHotel.Items;
using Akiled.HabboHotel.Rooms.Wired.WiredHandlers.Interfaces;

namespace Akiled.HabboHotel.Rooms.Wired.WiredHandlers
{
    public class Repeaterlong : IWired, IWiredCycleable
    {
        public int Delay { get; set; }
        private WiredHandler handler;
        private Item item;
        private bool disposed;

        public Repeaterlong(WiredHandler handler, Item item, int cyclesRequired)
        {
            this.handler = handler;
            this.Delay = cyclesRequired * 10;
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
            WiredUtillity.SaveTriggerItem(dbClient, this.item.Id, string.Empty, (this.Delay / 10).ToString(), false, null);
        }

        public void LoadFromDatabase(IQueryAdapter dbClient, Room insideRoom)
        {
            dbClient.SetQuery("SELECT trigger_data FROM wired_items WHERE trigger_id = @id");
            dbClient.AddParameter("id", this.item.Id);
            this.Delay = dbClient.GetInteger() * 10;
        }

        public void DeleteFromDatabase(IQueryAdapter dbClient)
        {
            dbClient.RunQuery("DELETE FROM wired_items WHERE trigger_id = '" + this.item.Id + "'");
        }

        public void OnTrigger(GameClient Session, int SpriteId)
        {
            ServerPacket Messagelong = new ServerPacket(ServerPacketHeader.WiredTriggerConfigMessageComposer);
            Messagelong.WriteBoolean(false);
            Messagelong.WriteInteger(5);
            Messagelong.WriteInteger(0);
            Messagelong.WriteInteger(SpriteId);
            Messagelong.WriteInteger(this.item.Id);
            Messagelong.WriteString("");
            Messagelong.WriteInteger(1);
            Messagelong.WriteInteger(this.Delay / 10);
            Messagelong.WriteInteger(0);
            Messagelong.WriteInteger(12);
            Messagelong.WriteInteger(0);
            Messagelong.WriteInteger(0);
            Session.SendPacket(Messagelong);
        }

        public bool Disposed()
        {
            return this.disposed;
        }

    }
}
