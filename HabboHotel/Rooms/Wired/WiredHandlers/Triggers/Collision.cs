using Akiled.Communication.Packets.Outgoing;
using Akiled.Database.Interfaces;
using Akiled.HabboHotel.GameClients;
using Akiled.HabboHotel.Items;
using Akiled.HabboHotel.Rooms.Wired.WiredHandlers.Interfaces;

namespace Akiled.HabboHotel.Rooms.Wired.WiredHandlers.Triggers
{
    public class Collision : IWired
    {
        private Item item;
        private WiredHandler handler;
        private readonly UserAndItemDelegate delegateFunction;
        private bool disposed;

        public Collision(Item item, WiredHandler handler, RoomUserManager roomUserManager)
        {
            this.item = item;
            this.handler = handler;
            this.delegateFunction = new UserAndItemDelegate(this.FurniCollision);
            this.handler.TrgCollision += this.delegateFunction;
        }

        private void FurniCollision(RoomUser user, Item item)
        {
            if (user == null)
                return;

            this.handler.ExecutePile(this.item.Coordinate, user, item);
        }

        public bool Disposed()
        {
            return this.disposed;
        }

        public void Dispose()
        {
            this.disposed = true;
            this.handler.TrgCollision -= this.delegateFunction;
            this.handler = (WiredHandler)null;
            this.item = (Item)null;
        }

        public void SaveToDatabase(IQueryAdapter dbClient)
        {
        }

        public void LoadFromDatabase(IQueryAdapter dbClient, Room insideRoom)
        {
        }

        public void DeleteFromDatabase(IQueryAdapter dbClient)
        {
        }

        public void OnTrigger(GameClient Session, int SpriteId)
        {
            ServerPacket Message = new ServerPacket(ServerPacketHeader.WiredTriggerConfigMessageComposer);
            Message.WriteBoolean(false);
            Message.WriteInteger(0);
            Message.WriteInteger(0);
            Message.WriteInteger(SpriteId);
            Message.WriteInteger(this.item.Id);
            Message.WriteString("");
            Message.WriteInteger(0);
            Message.WriteInteger(0);
            Message.WriteInteger(8);
            Message.WriteInteger(0);
            Message.WriteInteger(0);
            Message.WriteInteger(0);
            Session.SendPacket(Message);
        }
    }
}
