using Akiled.Communication.Packets.Outgoing;
using Akiled.Database.Interfaces;
using Akiled.HabboHotel.GameClients;
using Akiled.HabboHotel.Items;
using Akiled.HabboHotel.Rooms.Wired.WiredHandlers.Interfaces;
using System.Data;

namespace Akiled.HabboHotel.Rooms.Wired.WiredHandlers.Triggers
{
    public class UserSays : IWired
    {
        private Item item;
        private WiredHandler handler;
        private bool isOwnerOnly;
        private string triggerMessage;
        private readonly RoomUserSaysDelegate delegateFunction;

        public UserSays(Item item, WiredHandler handler, bool isOwnerOnly, string triggerMessage, Room room)
        {
            this.item = item;
            this.handler = handler;
            this.isOwnerOnly = isOwnerOnly;
            this.triggerMessage = triggerMessage;
            this.delegateFunction = new RoomUserSaysDelegate(this.OnUserSays);
            room.OnUserSays += this.delegateFunction;
        }

        private void OnUserSays(object sender, UserSaysArgs e, ref bool messageHandled)
        {
            RoomUser user = e.User;
            string message = e.Message;

            if (user != null && (!this.isOwnerOnly && this.canBeTriggered(message) && !string.IsNullOrEmpty(message)) || (this.isOwnerOnly && user.IsOwner() && this.canBeTriggered(message) && !string.IsNullOrEmpty(message)))
            {
                this.handler.ExecutePile(this.item.Coordinate, user, null);
                messageHandled = true;
            }
        }

        private bool canBeTriggered(string message)
        {
            if (string.IsNullOrEmpty(this.triggerMessage))
                return false;
            return message.ToLower() == this.triggerMessage.ToLower();
        }

        public void Dispose()
        {
            this.handler.GetRoom().OnUserSays -= this.delegateFunction;
            this.item = (Item)null;
            this.handler = (WiredHandler)null;
            this.triggerMessage = (string)null;
        }

        public void SaveToDatabase(IQueryAdapter dbClient)
        {
            WiredUtillity.SaveTriggerItem(dbClient, this.item.Id, string.Empty, this.triggerMessage, this.isOwnerOnly, null);
        }

        public void LoadFromDatabase(IQueryAdapter dbClient, Room insideRoom)
        {
            dbClient.SetQuery("SELECT trigger_data,  all_user_triggerable FROM wired_items WHERE trigger_id = @id ");
            dbClient.AddParameter("id", this.item.Id);
            DataRow row = dbClient.GetRow();
            if (row != null)
            {
                this.triggerMessage = row[0].ToString();
                this.isOwnerOnly = row[1].ToString() == "1";
            }
            else
            {
                this.triggerMessage = string.Empty;
                this.isOwnerOnly = false;
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
