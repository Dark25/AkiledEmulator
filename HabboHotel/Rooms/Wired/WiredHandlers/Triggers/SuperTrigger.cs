using Akiled.Communication.Packets.Outgoing;
using Akiled.Database.Interfaces;
using Akiled.HabboHotel.GameClients;
using Akiled.HabboHotel.Items;
using Akiled.HabboHotel.Rooms.Wired.WiredHandlers.Interfaces;
using System.Data;

namespace Akiled.HabboHotel.Rooms.Wired.WiredHandlers.Triggers
{
    public class SuperTrigger : IWired
    {
        private Item item;
        private WiredHandler handler;
        private string triggerMessage;
        private readonly TriggerUserDelegate delegateFunction;

        public SuperTrigger(Item item, WiredHandler handler, string triggerMessage, Room room)
        {
            switch (triggerMessage)
            {
                case "test":
                    this.triggerMessage = triggerMessage;
                    break;
            }

            this.item = item;
            this.handler = handler;
            this.delegateFunction = new TriggerUserDelegate(this.roomUserManager_SuperTrigger);
            room.TriggerUser += this.delegateFunction;
        }

        private void roomUserManager_SuperTrigger(RoomUser user, string ActionType)
        {
            if (ActionType == this.triggerMessage)
                this.handler.ExecutePile(this.item.Coordinate, user, null);
        }

        public void Dispose()
        {
            this.handler.GetRoom().TriggerUser -= this.delegateFunction;
            this.item = (Item)null;
            this.handler = (WiredHandler)null;
            this.triggerMessage = (string)null;
        }

        public void SaveToDatabase(IQueryAdapter dbClient)
        {
            WiredUtillity.SaveTriggerItem(dbClient, this.item.Id, string.Empty, this.triggerMessage, false, null);
        }

        public void LoadFromDatabase(IQueryAdapter dbClient, Room insideRoom)
        {
            dbClient.SetQuery("SELECT trigger_data,  all_user_triggerable FROM wired_items WHERE trigger_id = @id ");
            dbClient.AddParameter("id", this.item.Id);
            DataRow row = dbClient.GetRow();
            if (row != null)
            {
                this.triggerMessage = row[0].ToString();
            }
            else
            {
                this.triggerMessage = string.Empty;
            }
        }

        public void OnTrigger(GameClient Session, int SpriteId)
        {
            ServerPacket Message6 = new ServerPacket(ServerPacketHeader.WiredTriggerConfigMessageComposer);
            Message6.WriteBoolean(false);
            Message6.WriteInteger(0);
            Message6.WriteInteger(0);
            Message6.WriteInteger(SpriteId);
            Message6.WriteInteger(this.item.Id);
            Message6.WriteString(this.triggerMessage);
            Message6.WriteInteger(0);
            Message6.WriteInteger(0);
            Message6.WriteInteger(0);
            Message6.WriteInteger(0);
            Message6.WriteInteger(0);
            Message6.WriteInteger(0);
            Session.SendPacket(Message6);
        }

        public void DeleteFromDatabase(IQueryAdapter dbClient)
        {
            dbClient.RunQuery("DELETE FROM wired_items WHERE trigger_id = '" + this.item.Id + "'");
        }
    }
}
